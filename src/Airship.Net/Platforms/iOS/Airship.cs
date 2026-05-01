/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;
using Foundation;
using Airship;
using AirshipDotNet.Analytics;
using AirshipDotNet.Attributes;
using AirshipDotNet.Events;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Platforms.iOS;
using AirshipDotNet.Platforms.iOS.Modules;

namespace AirshipDotNet
{
    // Internal delegate classes for SDK 19 compatibility
    internal class AirshipDeepLinkDelegate : global::Airship.UADeepLinkDelegate
    {
        private readonly Action<string> handler;

        public AirshipDeepLinkDelegate(Action<string> handler)
        {
            this.handler = handler;
        }

        public override void CompletionHandler(NSUrl deepLink, Action completionHandler)
        {
            handler?.Invoke(deepLink.AbsoluteString!);
            completionHandler();
        }
    }


    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : NSObject
    {
        private static readonly Lazy<Airship> sharedAirship = new(() =>
        {
            Airship instance = new();
            instance.Initialize();
            return instance;
        });

        // Event streams similar to Flutter implementation
        private readonly Dictionary<AirshipEventType, AirshipEventStream> _eventStreams;

        // Handler mappings to prevent memory leaks
        private readonly Dictionary<EventHandler<ChannelEventArgs>, EventHandler<EventArgs>> _channelHandlerMap = new();
        private readonly Dictionary<EventHandler<PushNotificationStatusEventArgs>, EventHandler<EventArgs>> _pushStatusHandlerMap = new();
        private readonly Dictionary<EventHandler<DeepLinkEventArgs>, EventHandler<EventArgs>> _deepLinkHandlerMap = new();

        // Module instances
        private readonly AirshipModule _module;
        private readonly IAirshipPush _push;
        private readonly IAirshipChannel _channel;
        private readonly IAirshipContact _contact;
        private readonly IAirshipAnalytics _analytics;
        private readonly IAirshipInApp _inApp;
        private readonly IAirshipPrivacyManager _privacyManager;
        private readonly IAirshipFeatureFlagManager _featureFlagManager;
        private readonly IAirshipPreferenceCenter _preferenceCenter;
        private readonly IAirshipMessageCenter _messageCenter;
        private readonly IAirshipPermissionsManager _permissionsManager;

        public Airship()
        {
            // Initialize event streams
            _eventStreams = AirshipEventStream.GenerateEventStreams();

            _module = new AirshipModule();
            _push = new AirshipPush(_module);
            _channel = new AirshipChannel(_module);
            _contact = new AirshipContact(_module);
            _analytics = new AirshipAnalytics(_module);
            _inApp = new AirshipInApp(_module);
            _privacyManager = new AirshipPrivacyManager(_module);
            _featureFlagManager = new AirshipFeatureFlagManager(_module);
            _preferenceCenter = new AirshipPreferenceCenter(_module);
            _messageCenter = new AirshipDotNet.Platforms.iOS.Modules.AirshipMessageCenter(_module);
            _permissionsManager = new AirshipDotNet.Platforms.iOS.Modules.AirshipPermissionsManager(_module);
        }

        private void Initialize()
        {
            // Channel creation notification
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)UAirshipNotificationChannelCreated.Name, (notification) =>
            {
                string channelID = notification.UserInfo?[UAirshipNotificationChannelCreated.ChannelIDKey]?.ToString() ?? "";
                var eventArgs = new ChannelEventArgs(channelID);

                AirshipEventEmitter.Shared.Emit(AirshipEventType.ChannelCreated, eventArgs);
            });

            // Message Center updated notification
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)"com.urbanairship.notification.message_list_updated", (notification) =>
            {
                OnMessagesUpdated?.Invoke(this, new EventArgs());
            });

            // Subscribe to pending events when listeners are added
            AirshipEventEmitter.Shared.PendingEventAvailable += OnPendingEventAvailable;
        }

        private void OnPendingEventAvailable(object? sender, AirshipEventType eventType)
        {
            // Process pending events through the stream
            if (_eventStreams.TryGetValue(eventType, out var stream))
            {
                _ = stream.ProcessPendingEvents();
            }
        }

        /// <summary>
        /// Add/remove the Message Center updated listener.
        /// </summary>
        internal event EventHandler<EventArgs>? OnMessagesUpdated;

        /// <summary>
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation
        {
            add
            {
                if (value != null)
                {
                    // Create and store wrapper handler to prevent memory leak
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, args as ChannelEventArgs ?? new ChannelEventArgs(""));

                    _channelHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.ChannelCreated, wrapper);
                }
            }
            remove
            {
                if (value != null && _channelHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.ChannelCreated, wrapper);
                    _channelHandlerMap.Remove(value);
                }
            }
        }

        /// <summary>
        /// Add/remove the push notification status listener.
        /// </summary>
        public event EventHandler<PushNotificationStatusEventArgs>? OnPushNotificationStatusUpdate
        {
            add
            {
                if (value != null)
                {
                    // Create and store wrapper handler to prevent memory leak
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, args as PushNotificationStatusEventArgs ??
                            new PushNotificationStatusEventArgs(new PushNotificationStatus()));

                    _pushStatusHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.NotificationStatusChanged, wrapper);
                }
            }
            remove
            {
                if (value != null && _pushStatusHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.NotificationStatusChanged, wrapper);
                    _pushStatusHandlerMap.Remove(value);
                }
            }
        }

        private AirshipDeepLinkDelegate? deepLinkDelegate;

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                if (value != null)
                {
                    // Create and store wrapper handler to prevent memory leak
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, args as DeepLinkEventArgs ?? new DeepLinkEventArgs(""));

                    _deepLinkHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.DeepLinkReceived, wrapper);
                }

                if (deepLinkDelegate == null)
                {
                    deepLinkDelegate = new AirshipDeepLinkDelegate((deepLink) =>
                    {
                        var eventArgs = new DeepLinkEventArgs(deepLink);

                        AirshipEventEmitter.Shared.Emit(AirshipEventType.DeepLinkReceived, eventArgs);
                    });
                    UAirship.DeepLinkDelegate = deepLinkDelegate;
                }
            }
            remove
            {
                if (value != null && _deepLinkHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.DeepLinkReceived, wrapper);
                    _deepLinkHandlerMap.Remove(value);
                }

                if (_deepLinkHandlerMap.Count == 0)
                {
                    UAirship.DeepLinkDelegate = null;
                    deepLinkDelegate = null;
                }
            }
        }

        // Internal delegate class for Message Center display
        internal class AirshipMessageCenterDisplayDelegate : global::Airship.UAMessageCenterDisplayDelegate
        {
            private readonly Action<string?> handler;

            public AirshipMessageCenterDisplayDelegate(Action<string?> handler)
            {
                this.handler = handler;
            }

            public override void DisplayMessageCenterForMessageID(string messageId)
            {
                handler?.Invoke(messageId);
            }

            public override void DisplayMessageCenter()
            {
                handler?.Invoke(null);
            }

            public override void DismissMessageCenter()
            {
                handler?.Invoke(null);
            }
        }

        private EventHandler<MessageCenterEventArgs>? onMessageCenterDisplay;
        private AirshipMessageCenterDisplayDelegate? messageCenterDisplayDelegate;

        /// <summary>
        /// Add/remove the Message Center display listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs> OnMessageCenterDisplay
        {
            add
            {
                onMessageCenterDisplay += value;
                if (messageCenterDisplayDelegate == null)
                {
                    messageCenterDisplayDelegate = new AirshipMessageCenterDisplayDelegate((messageId) =>
                    {
                        onMessageCenterDisplay?.Invoke(this, new MessageCenterEventArgs(messageId));
                    });
                    UAirship.MessageCenter.WeakDisplayDelegate = messageCenterDisplayDelegate;
                }
            }
            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    UAirship.MessageCenter.WeakDisplayDelegate = null;
                    messageCenterDisplayDelegate = null;
                }
            }
        }


        public static Airship Instance => sharedAirship.Value;

        /// <summary>
        /// Gets the Airship .NET library version.
        /// </summary>
        public static string Version => "21.4.0";

        // Module properties
        public static IAirshipPush Push => Instance._push;
        public static IAirshipChannel Channel => Instance._channel;
        public static IAirshipContact Contact => Instance._contact;
        public static IAirshipAnalytics Analytics => Instance._analytics;
        public static IAirshipInApp InApp => Instance._inApp;
        public static IAirshipPrivacyManager PrivacyManager => Instance._privacyManager;
        public static IAirshipFeatureFlagManager FeatureFlagManager => Instance._featureFlagManager;
        public static IAirshipPreferenceCenter PreferenceCenter => Instance._preferenceCenter;
        public static IAirshipMessageCenter MessageCenter => Instance._messageCenter;
        public static IAirshipPermissionsManager PermissionsManager => Instance._permissionsManager;

        /// <summary>
        /// Processes a deep link.
        /// </summary>
        /// <param name="url">The deep link URL.</param>
        /// <returns>A task that completes with true if the deep link was handled, false otherwise.</returns>
        public static Task<bool> ProcessDeepLink(string url)
        {
            var tcs = new TaskCompletionSource<bool>();
            var nsUrl = new Foundation.NSUrl(url);
            UAirship.ProcessDeepLink(nsUrl, (handled) =>
            {
                tcs.TrySetResult(handled);
            });
            return tcs.Task;
        }

    }
}