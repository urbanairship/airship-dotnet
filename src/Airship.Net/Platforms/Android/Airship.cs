/* Copyright Airship and Contributors */

using Com.Urbanairship.Contacts;
using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
using UrbanAirship.MessageCenter;
using UrbanAirship.Push;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : Java.Lang.Object, IDeepLinkListener, IInboxListener, MessageCenterClass.IOnShowMessageCenterListener, UrbanAirship.Channel.IAirshipChannelListener, IPushNotificationStatusListener
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
        private readonly IAirshipMessageCenter _messageCenter;
        private readonly IAirshipAnalytics _analytics;
        private readonly IAirshipInApp _inApp;
        private readonly IAirshipPrivacyManager _privacyManager;

        public Airship()
        {
            _module = new AirshipModule();
            _push = new AirshipPush(_module);
            _channel = new AirshipChannel(_module);
            _contact = new AirshipContact(_module);
            _messageCenter = new AirshipMessageCenter(_module);
            _analytics = new AirshipAnalytics(_module);
            _inApp = new AirshipInApp(_module);
            _privacyManager = new AirshipPrivacyManager(_module);
        }

        private void Init()
        {
            UAirship.Shared().Channel.AddChannelListener(this);

            //Adding Push notification status listener
            UAirship.Shared().PushManager.AddNotificationStatusListener(this);

            //Adding Inbox updated listener
            MessageCenterClass.Shared().Inbox.AddListener(this);
        }

        /// <summary>
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation;

        /// <summary>
        /// Add/remove the push notification status listener.
        /// </summary>
        public event EventHandler<PushNotificationStatusEventArgs>? OnPushNotificationStatusUpdate;

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

        /// <summary>
        /// Add/remove the Message Center updated listener.
        /// </summary>
        public event EventHandler? OnMessageCenterUpdated;

        private EventHandler<MessageCenterEventArgs>? onMessageCenterDisplay;

        /// <summary>
        /// Add/remove the Message Center display listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs> OnMessageCenterDisplay
        {
            add
            {
                onMessageCenterDisplay += value;
                // TODO: SetOnShowMessageCenterListener is not exposed in the Android bindings.
                // The method is marked as kotlin-internal in the native SDK and needs metadata
                // transformation to expose it. See binderator/.../Metadata.xml for fix.
                //MessageCenterClass.Shared().SetOnShowMessageCenterListener(this);
            }

            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    // TODO: SetOnShowMessageCenterListener is not exposed in the Android bindings.
                    // See comment above for details.
                    // MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
                }
            }
        }

        public static Airship Instance => sharedAirship.Value;

        // Module properties
        public static IAirshipPush Push => Instance._push;
        public static IAirshipChannel Channel => Instance._channel;
        public static IAirshipContact Contact => Instance._contact;
        public static IAirshipMessageCenter MessageCenter => Instance._messageCenter;
        public static IAirshipAnalytics Analytics => Instance._analytics;
        public static IAirshipInApp InApp => Instance._inApp;
        public static IAirshipPrivacyManager PrivacyManager => Instance._privacyManager;

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

        public bool OnShowMessageCenter(string? messageId)
        {
            if (onMessageCenterDisplay != null)
            {
                onMessageCenterDisplay(this, new MessageCenterEventArgs(messageId));
                return true;
            }

            return false;
        }

        public void OnInboxUpdated() => OnMessageCenterUpdated?.Invoke(this, EventArgs.Empty);

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
    }
}