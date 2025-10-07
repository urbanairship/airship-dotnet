/* Copyright Airship and Contributors */

using Foundation;
using Airship;
using AirshipDotNet.Analytics;
using AirshipDotNet.Attributes;
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
                OnChannelCreation?.Invoke(this, new ChannelEventArgs(channelID));
            });

            // Note: Push notification status update event is not directly available in SDK 19
            // This functionality may need to be implemented differently using the new SDK APIs

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
        private AirshipDeepLinkDelegate? deepLinkDelegate;

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                onDeepLinkReceived += value;
                if (deepLinkDelegate == null)
                {
                    deepLinkDelegate = new AirshipDeepLinkDelegate((deepLink) =>
                    {
                        onDeepLinkReceived?.Invoke(this, new DeepLinkEventArgs(deepLink));
                    });
                    UAirship.DeepLinkDelegate = deepLinkDelegate;
                }
            }
            remove
            {
                onDeepLinkReceived -= value;

                if (onDeepLinkReceived == null)
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