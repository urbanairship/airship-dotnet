/* Copyright Airship and Contributors */

using Com.Urbanairship.Contacts;
using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
using UrbanAirship.Push;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Platforms.Android;
using AirshipDotNet.Platforms.Android.Modules;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : Java.Lang.Object, IDeepLinkListener, UrbanAirship.Channel.IAirshipChannelListener, IPushNotificationStatusListener, UrbanAirship.MessageCenter.IInboxListener
    {
        private static readonly Lazy<Airship> sharedAirship = new(() =>
        {
            Airship instance = new();
            instance.Init();
            return instance;
        });

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

            //Adding Push notification status listener
            UAirship.Shared().PushManager.AddNotificationStatusListener(this);

            UrbanAirship.MessageCenter.MessageCenterClass.Shared().Inbox.AddListener(this);

        }

        /// <summary>
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation;

        /// <summary>
        /// Add/remove the push notification status listener.
        /// </summary>
        public event EventHandler<PushNotificationStatusEventArgs>? OnPushNotificationStatusUpdate;

        /// <summary>
        /// Add/remove the Message Center updated listener.
        /// </summary>
        internal event EventHandler<EventArgs>? OnMessagesUpdated;

        private EventHandler<DeepLinkEventArgs>? onDeepLinkReceived;

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                onDeepLinkReceived += value;
                UAirship.Shared().DeepLinkListener = this;
            }

            remove
            {
                onDeepLinkReceived -= value;

                if (onDeepLinkReceived == null)
                {
                    UAirship.Shared().DeepLinkListener = null;
                }
            }
        }

        // Internal delegate class for Message Center display
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
                    UrbanAirship.MessageCenter.MessageCenterClass.Shared().SetOnShowMessageCenterListener(messageCenterDisplayDelegate);
                }
            }
            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    UrbanAirship.MessageCenter.MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
                    messageCenterDisplayDelegate = null;
                }
            }
        }


        public static Airship Instance => sharedAirship.Value;

        /// <summary>
        /// Gets the Airship .NET library version.
        /// </summary>
        public static string Version => "20.2.1";

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
            if (onDeepLinkReceived != null)
            {
                onDeepLinkReceived(this, new DeepLinkEventArgs(deepLink));
                return true;
            }

            return false;
        }


        public void OnChannelCreated(string channelId) => OnChannelCreation?.Invoke(this, new ChannelEventArgs(channelId));

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

            OnPushNotificationStatusUpdate?.Invoke(this, new PushNotificationStatusEventArgs(pushStatus));
        }

        public void OnInboxUpdated() => OnMessagesUpdated?.Invoke(this, new EventArgs());
    }
}