/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;
using Foundation;
using Airship;
using AirshipDotNet.Analytics;
using AirshipDotNet.Attributes;
using AirshipDotNet.Events;
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
        }

        private void Initialize()
        {
            // Channel creation notification
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)UAirshipNotificationChannelCreated.Name, (notification) =>
            {
                string channelID = notification.UserInfo?[UAirshipNotificationChannelCreated.ChannelIDKey]?.ToString() ?? "";
                var eventArgs = new ChannelEventArgs(channelID);

                // Emit to event queue
                AirshipEventEmitter.Shared.Emit(AirshipEventType.ChannelCreated, eventArgs);

                // Also fire traditional event for backwards compatibility
                _onChannelCreation?.Invoke(this, eventArgs);
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

        private EventHandler<ChannelEventArgs>? _onChannelCreation;
        /// <summary>
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation
        {
            add
            {
                _onChannelCreation += value;
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
                _onChannelCreation -= value;
                if (value != null && _channelHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.ChannelCreated, wrapper);
                    _channelHandlerMap.Remove(value);
                }
            }
        }

        private EventHandler<PushNotificationStatusEventArgs>? _onPushNotificationStatusUpdate;
        /// <summary>
        /// Add/remove the push notification status listener.
        /// </summary>
        public event EventHandler<PushNotificationStatusEventArgs>? OnPushNotificationStatusUpdate
        {
            add
            {
                _onPushNotificationStatusUpdate += value;
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
                _onPushNotificationStatusUpdate -= value;
                if (value != null && _pushStatusHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.NotificationStatusChanged, wrapper);
                    _pushStatusHandlerMap.Remove(value);
                }
            }
        }

        private EventHandler<DeepLinkEventArgs>? onDeepLinkReceived;
        private AirshipDeepLinkDelegate? deepLinkDelegate;

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                onDeepLinkReceived += value;
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

                        // Emit to event queue
                        AirshipEventEmitter.Shared.Emit(AirshipEventType.DeepLinkReceived, eventArgs);

                        // Also fire traditional event for backwards compatibility
                        onDeepLinkReceived?.Invoke(this, eventArgs);
                    });
                    UAirship.DeepLinkDelegate = deepLinkDelegate;
                }
            }
            remove
            {
                onDeepLinkReceived -= value;
                if (value != null && _deepLinkHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.DeepLinkReceived, wrapper);
                    _deepLinkHandlerMap.Remove(value);
                }

                if (onDeepLinkReceived == null)
                {
                    UAirship.DeepLinkDelegate = null;
                    deepLinkDelegate = null;
                }
            }
        }


        public static Airship Instance => sharedAirship.Value;

        // Module properties
        public static IAirshipPush Push => Instance._push;
        public static IAirshipChannel Channel => Instance._channel;
        public static IAirshipContact Contact => Instance._contact;
        public static IAirshipAnalytics Analytics => Instance._analytics;
        public static IAirshipInApp InApp => Instance._inApp;
        public static IAirshipPrivacyManager PrivacyManager => Instance._privacyManager;
        public static IAirshipFeatureFlagManager FeatureFlagManager => Instance._featureFlagManager;
        public static IAirshipPreferenceCenter PreferenceCenter => Instance._preferenceCenter;
    }
}