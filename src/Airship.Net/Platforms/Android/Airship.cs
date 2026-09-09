/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Com.Urbanairship.Contacts;
using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
using UrbanAirship.Push;
using UrbanAirship.PreferenceCenter;
using AirshipDotNet.Events;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Platforms.Android;
using AirshipDotNet.Platforms.Android.Modules;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : Java.Lang.Object,
        IDeepLinkListener,
        UrbanAirship.Channel.IAirshipChannelListener,
        IPushNotificationStatusListener,
        UrbanAirship.MessageCenter.IInboxListener,
        IPushTokenListener,
        IPushListener,
        INotificationListener
    {
        private static readonly Lazy<Airship> sharedAirship = new(() =>
        {
            Airship instance = new();
            instance.Init();
            return instance;
        });

        // Event streams similar to Flutter implementation
        private readonly Dictionary<AirshipEventType, AirshipEventStream> _eventStreams;

        // Handler mappings to prevent memory leaks
        private readonly Dictionary<EventHandler<ChannelEventArgs>, EventHandler<EventArgs>> _channelHandlerMap = new();
        private readonly Dictionary<EventHandler<PushNotificationStatusEventArgs>, EventHandler<EventArgs>> _pushStatusHandlerMap = new();
        private readonly Dictionary<EventHandler<DeepLinkEventArgs>, EventHandler<EventArgs>> _deepLinkHandlerMap = new();
        private readonly Dictionary<EventHandler<PushReceivedEventArgs>, EventHandler<EventArgs>> _pushReceivedHandlerMap = new();
        private readonly Dictionary<EventHandler<NotificationResponseEventArgs>, EventHandler<EventArgs>> _notificationResponseHandlerMap = new();
        private readonly Dictionary<EventHandler<PushTokenReceivedEventArgs>, EventHandler<EventArgs>> _pushTokenHandlerMap = new();
        private readonly Dictionary<EventHandler<MessageCenterEventArgs>, EventHandler<EventArgs>> _displayMessageCenterHandlerMap = new();
        private readonly Dictionary<EventHandler<EventArgs>, EventHandler<EventArgs>> _messageCenterUpdatedHandlerMap = new();
        private readonly Dictionary<EventHandler<PreferenceCenterEventArgs>, EventHandler<EventArgs>> _displayPreferenceCenterHandlerMap = new();

        // Strong references for delegates / listeners that are held weakly or by interface
        private AirshipMessageCenterDisplayDelegate? _messageCenterDisplayDelegate;
        private AirshipPreferenceCenterOpenDelegate? _preferenceCenterOpenDelegate;
        private global::Com.Urbanairship.Embedded.AirshipEmbeddedObserver? _embeddedObserver;
        private AirshipEmbeddedObserverListener? _embeddedObserverListener;
        private readonly Android.OS.Handler _mainHandler = new Android.OS.Handler(Android.OS.Looper.MainLooper!);

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
            _channel = new AirshipDotNet.Platforms.Android.Modules.AirshipChannel(_module);
            _contact = new AirshipContact(_module);
            _analytics = new AirshipAnalytics(_module);
            _inApp = new AirshipInApp(_module);
            _privacyManager = new AirshipPrivacyManager(_module);
            _featureFlagManager = new AirshipFeatureFlagManager(_module);
            _preferenceCenter = new AirshipPreferenceCenter(_module);
            _messageCenter = new AirshipDotNet.Platforms.Android.Modules.AirshipMessageCenter(_module);
            _permissionsManager = new AirshipDotNet.Platforms.Android.Modules.AirshipPermissionsManager(_module);
        }

        private void Init()
        {
            UAirship.Shared().Channel.AddChannelListener(this);
            UAirship.Shared().PushManager.AddNotificationStatusListener(this);
            UAirship.Shared().PushManager.AddPushTokenListener(this);
            UAirship.Shared().PushManager.AddPushListener(this);
            UAirship.Shared().PushManager.NotificationListener = this;

            UrbanAirship.MessageCenter.MessageCenterClass.Shared().Inbox.AddListener(this);

            // Subscribe to pending events when listeners are added
            AirshipEventEmitter.Shared.PendingEventAvailable += OnPendingEventAvailable;

            StartEmbeddedObserver();
        }

        private void StartEmbeddedObserver()
        {
            // The vararg constructor filters to the given IDs, so an empty array would match
            // nothing; observe all embedded IDs with an always-true filter instead.
            _embeddedObserver = new global::Com.Urbanairship.Embedded.AirshipEmbeddedObserver(new AllEmbeddedIdsFilter());
            _embeddedObserverListener = new AirshipEmbeddedObserverListener((views) =>
            {
                var list = new List<EmbeddedInfo>();
                foreach (var info in views)
                {
                    list.Add(new EmbeddedInfo(info.EmbeddedId, info.InstanceId, info.Priority));
                }
                // The observer delivers updates on a background dispatcher; subscribers
                // (handlers, app code) touch UI state, so marshal to the main thread.
                _mainHandler.Post(() => UpdatePendingEmbedded(list));
            });
            _embeddedObserver.Listener = _embeddedObserverListener;
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
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation
        {
            add
            {
                if (value != null)
                {
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

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, args as DeepLinkEventArgs ?? new DeepLinkEventArgs(""));

                    _deepLinkHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.DeepLinkReceived, wrapper);
                }
                UAirship.Shared().DeepLinkListener = this;
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
                    UAirship.Shared().DeepLinkListener = null;
                }
            }
        }

        /// <summary>
        /// Add/remove the push received listener. Fires for both foreground and background pushes.
        /// </summary>
        public event EventHandler<PushReceivedEventArgs>? OnPushReceived
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, (args as PushReceivedEventArgs)!);

                    _pushReceivedHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.PushReceived, wrapper);
                }
            }
            remove
            {
                if (value != null && _pushReceivedHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.PushReceived, wrapper);
                    _pushReceivedHandlerMap.Remove(value);
                }
            }
        }

        /// <summary>
        /// Add/remove the notification response listener (user tap or action button).
        /// </summary>
        public event EventHandler<NotificationResponseEventArgs>? OnNotificationResponse
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, (args as NotificationResponseEventArgs)!);

                    _notificationResponseHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.NotificationResponse, wrapper);
                }
            }
            remove
            {
                if (value != null && _notificationResponseHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.NotificationResponse, wrapper);
                    _notificationResponseHandlerMap.Remove(value);
                }
            }
        }

        /// <summary>
        /// Add/remove the push token received listener.
        /// </summary>
        public event EventHandler<PushTokenReceivedEventArgs>? OnPushTokenReceived
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, (args as PushTokenReceivedEventArgs)!);

                    _pushTokenHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.PushTokenReceived, wrapper);
                }
            }
            remove
            {
                if (value != null && _pushTokenHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.PushTokenReceived, wrapper);
                    _pushTokenHandlerMap.Remove(value);
                }
            }
        }

        /// <summary>
        /// Add/remove the message center display request listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs>? OnDisplayMessageCenter
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, args as MessageCenterEventArgs ?? new MessageCenterEventArgs(null));

                    _displayMessageCenterHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.DisplayMessageCenter, wrapper);

                    if (_messageCenterDisplayDelegate == null)
                    {
                        _messageCenterDisplayDelegate = new AirshipMessageCenterDisplayDelegate((messageId) =>
                        {
                            AirshipEventEmitter.Shared.Emit(AirshipEventType.DisplayMessageCenter, new MessageCenterEventArgs(messageId));
                        });
                        UrbanAirship.MessageCenter.MessageCenterClass.Shared().SetOnShowMessageCenterListener(_messageCenterDisplayDelegate);
                    }
                }
            }
            remove
            {
                if (value != null && _displayMessageCenterHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.DisplayMessageCenter, wrapper);
                    _displayMessageCenterHandlerMap.Remove(value);
                }

                if (_displayMessageCenterHandlerMap.Count == 0 && _messageCenterDisplayDelegate != null)
                {
                    UrbanAirship.MessageCenter.MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
                    _messageCenterDisplayDelegate = null;
                }
            }
        }

        private IReadOnlyList<EmbeddedInfo> _pendingEmbedded = new List<EmbeddedInfo>();

        /// <summary>Latest snapshot of pending embedded content.</summary>
        internal IReadOnlyList<EmbeddedInfo> PendingEmbedded => _pendingEmbedded;

        /// <summary>Raised when the pending embedded snapshot changes.</summary>
        internal event EventHandler<EmbeddedInfoUpdatedEventArgs>? OnEmbeddedInfoUpdated;

        /// <summary>Updates the cached snapshot, raises the facade event, and emits the SDK event.</summary>
        internal void UpdatePendingEmbedded(IReadOnlyList<EmbeddedInfo> pending)
        {
            _pendingEmbedded = pending;
            var args = new EmbeddedInfoUpdatedEventArgs(pending);
            OnEmbeddedInfoUpdated?.Invoke(this, args);
            AirshipDotNet.Events.AirshipEventEmitter.Shared.Emit(
                AirshipDotNet.Events.AirshipEventType.PendingEmbeddedUpdated, args);
        }

        /// <summary>
        /// Add/remove the message center inbox updated listener.
        /// </summary>
        public event EventHandler<EventArgs>? OnMessageCenterUpdated
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) => value(this, args);

                    _messageCenterUpdatedHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.MessageCenterUpdated, wrapper);
                }
            }
            remove
            {
                if (value != null && _messageCenterUpdatedHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.MessageCenterUpdated, wrapper);
                    _messageCenterUpdatedHandlerMap.Remove(value);
                }
            }
        }

        /// <summary>
        /// Add/remove the preference center display request listener.
        /// </summary>
        public event EventHandler<PreferenceCenterEventArgs>? OnDisplayPreferenceCenter
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, (args as PreferenceCenterEventArgs)!);

                    _displayPreferenceCenterHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.DisplayPreferenceCenter, wrapper);

                    if (_preferenceCenterOpenDelegate == null)
                    {
                        _preferenceCenterOpenDelegate = new AirshipPreferenceCenterOpenDelegate((preferenceCenterId) =>
                        {
                            AirshipEventEmitter.Shared.Emit(AirshipEventType.DisplayPreferenceCenter, new PreferenceCenterEventArgs(preferenceCenterId));
                        });
                        global::UrbanAirship.PreferenceCenter.PreferenceCenter.Shared().OpenListener = _preferenceCenterOpenDelegate;
                    }
                }
            }
            remove
            {
                if (value != null && _displayPreferenceCenterHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.DisplayPreferenceCenter, wrapper);
                    _displayPreferenceCenterHandlerMap.Remove(value);
                }

                if (_displayPreferenceCenterHandlerMap.Count == 0 && _preferenceCenterOpenDelegate != null)
                {
                    global::UrbanAirship.PreferenceCenter.PreferenceCenter.Shared().OpenListener = null;
                    _preferenceCenterOpenDelegate = null;
                }
            }
        }


        public static Airship Instance => sharedAirship.Value;

        /// <summary>
        /// Gets the Airship .NET library version.
        /// </summary>
        public static string Version => "21.5.0";

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
        /// For uairship:// scheme URLs, Airship will handle the deep link internally.
        /// For other URLs, Airship will forward the deep link to the deep link listener if set.
        /// </summary>
        /// <param name="url">The deep link URL.</param>
        /// <returns>True if the deep link was handled, false otherwise.</returns>
        public static Task<bool> ProcessDeepLink(string url)
        {
            var result = UAirship.Shared().DeepLink(url);
            return Task.FromResult(result);
        }

        // Interface implementations
        public bool OnDeepLink(string deepLink)
        {
            AirshipEventEmitter.Shared.Emit(AirshipEventType.DeepLinkReceived, new DeepLinkEventArgs(deepLink));
            return _deepLinkHandlerMap.Count > 0;
        }

        public void OnChannelCreated(string channelId)
        {
            AirshipEventEmitter.Shared.Emit(AirshipEventType.ChannelCreated, new ChannelEventArgs(channelId));
        }

        public void OnChange(UrbanAirship.Push.PushNotificationStatus status)
        {
            var pushStatus = new PushNotificationStatus
            {
                IsUserNotificationsEnabled = status.IsUserNotificationsEnabled,
                AreNotificationsAllowed = status.AreNotificationsAllowed,
                IsPushPrivacyFeatureEnabled = status.IsPushPrivacyFeatureEnabled,
                IsPushTokenRegistered = status.IsPushTokenRegistered,
                IsUserOptedIn = status.IsUserOptedIn,
                IsOptIn = status.IsOptIn
            };

            AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationStatusChanged, new PushNotificationStatusEventArgs(pushStatus));
        }

        public void OnInboxUpdated()
        {
            AirshipEventEmitter.Shared.Emit(AirshipEventType.MessageCenterUpdated, EventArgs.Empty);
        }

        public void OnPushTokenUpdated(string token)
        {
            AirshipEventEmitter.Shared.Emit(AirshipEventType.PushTokenReceived, new PushTokenReceivedEventArgs(token));
        }

        void IPushListener.OnPushReceived(PushMessage message, bool notificationPosted)
        {
            var args = BuildPushReceivedEventArgs(message, isBackground: !notificationPosted);
            AirshipEventEmitter.Shared.Emit(AirshipEventType.PushReceived, args);
        }

        public void OnNotificationPosted(NotificationInfo notificationInfo) { }

        public bool OnNotificationOpened(NotificationInfo notificationInfo)
        {
            var push = BuildPushReceivedEventArgs(notificationInfo.Message, isBackground: false);
            AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationResponse,
                new NotificationResponseEventArgs(push, actionId: null, isForeground: true));
            return false;
        }

        public bool OnNotificationForegroundAction(NotificationInfo notificationInfo, NotificationActionButtonInfo actionButtonInfo)
        {
            var push = BuildPushReceivedEventArgs(notificationInfo.Message, isBackground: false);
            AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationResponse,
                new NotificationResponseEventArgs(push, actionButtonInfo.ButtonId, isForeground: true));
            return false;
        }

        public void OnNotificationBackgroundAction(NotificationInfo notificationInfo, NotificationActionButtonInfo actionButtonInfo)
        {
            var push = BuildPushReceivedEventArgs(notificationInfo.Message, isBackground: false);
            AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationResponse,
                new NotificationResponseEventArgs(push, actionButtonInfo.ButtonId, isForeground: false));
        }

        public void OnNotificationDismissed(NotificationInfo notificationInfo) { }

        private static PushReceivedEventArgs BuildPushReceivedEventArgs(PushMessage message, bool isBackground)
        {
            var payload = BundleToDictionary(message.PushBundle);
            return new PushReceivedEventArgs(payload, message.Alert, message.Title, isBackground);
        }

        private static IDictionary<string, object?> BundleToDictionary(Bundle? bundle)
        {
            var dict = new Dictionary<string, object?>();
            if (bundle == null) return dict;

            foreach (var key in bundle.KeySet() ?? new List<string>())
            {
                dict[key] = bundle.Get(key)?.ToString();
            }
            return dict;
        }

        // Inner delegate classes wrapping Airship Java listener interfaces.

        internal class AirshipMessageCenterDisplayDelegate : Java.Lang.Object, UrbanAirship.MessageCenter.MessageCenterClass.IOnShowMessageCenterListener
        {
            private readonly Action<string?> handler;

            public AirshipMessageCenterDisplayDelegate(Action<string?> handler)
            {
                this.handler = handler;
            }

            public bool OnShowMessageCenter(string? messageId)
            {
                handler?.Invoke(messageId);
                return true;
            }
        }

        internal class AirshipPreferenceCenterOpenDelegate : Java.Lang.Object, global::UrbanAirship.PreferenceCenter.PreferenceCenter.IOnOpenListener
        {
            private readonly Action<string> handler;

            public AirshipPreferenceCenterOpenDelegate(Action<string> handler)
            {
                this.handler = handler;
            }

            public bool OnOpenPreferenceCenter(string preferenceCenterId)
            {
                handler?.Invoke(preferenceCenterId);
                return true;
            }
        }

        /// <summary>Always-true embedded info filter, so the observer reports all embedded IDs.</summary>
        internal class AllEmbeddedIdsFilter : Java.Lang.Object, Kotlin.Jvm.Functions.IFunction1
        {
            public Java.Lang.Object? Invoke(Java.Lang.Object? p0) => Java.Lang.Boolean.True;
        }

        internal class AirshipEmbeddedObserverListener : Java.Lang.Object, global::Com.Urbanairship.Embedded.AirshipEmbeddedObserver.IListener
        {
            private readonly Action<IList<global::Com.Urbanairship.Embedded.AirshipEmbeddedInfo>> _onUpdate;

            public AirshipEmbeddedObserverListener(Action<IList<global::Com.Urbanairship.Embedded.AirshipEmbeddedInfo>> onUpdate)
            {
                _onUpdate = onUpdate;
            }

            public void OnEmbeddedViewInfoUpdate(IList<global::Com.Urbanairship.Embedded.AirshipEmbeddedInfo> views)
            {
                _onUpdate?.Invoke(views);
            }
        }
    }
}
