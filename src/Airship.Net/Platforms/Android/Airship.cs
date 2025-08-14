/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;
using Com.Urbanairship.Contacts;
using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
using UrbanAirship.Push;
using AirshipDotNet.Events;
using AirshipDotNet.Platforms.Android;
using AirshipDotNet.Platforms.Android.Modules;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : Java.Lang.Object, IDeepLinkListener, UrbanAirship.Channel.IAirshipChannelListener, IPushNotificationStatusListener
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
            _channel = new AirshipDotNet.Platforms.Android.Modules.AirshipChannel(_module);
            _contact = new AirshipContact(_module);
            _analytics = new AirshipAnalytics(_module);
            _inApp = new AirshipInApp(_module);
            _privacyManager = new AirshipPrivacyManager(_module);
            _featureFlagManager = new AirshipFeatureFlagManager(_module);
            _preferenceCenter = new AirshipPreferenceCenter(_module);
        }

        private void Init()
        {
            UAirship.Shared().Channel.AddChannelListener(this);

            //Adding Push notification status listener
            UAirship.Shared().PushManager.AddNotificationStatusListener(this);

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
                UAirship.Shared().DeepLinkListener = this;
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
                    UAirship.Shared().DeepLinkListener = null;
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

        // Interface implementations
        public bool OnDeepLink(string deepLink)
        {
            var eventArgs = new DeepLinkEventArgs(deepLink);

            // Emit to event queue
            AirshipEventEmitter.Shared.Emit(AirshipEventType.DeepLinkReceived, eventArgs);

            // Also fire traditional event for backwards compatibility
            if (onDeepLinkReceived != null)
            {
                onDeepLinkReceived(this, eventArgs);
                return true;
            }

            return false;
        }


        public void OnChannelCreated(string channelId)
        {
            var eventArgs = new ChannelEventArgs(channelId);

            // Emit to event queue
            AirshipEventEmitter.Shared.Emit(AirshipEventType.ChannelCreated, eventArgs);

            // Also fire traditional event for backwards compatibility
            _onChannelCreation?.Invoke(this, eventArgs);
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

            var eventArgs = new PushNotificationStatusEventArgs(pushStatus);

            // Emit to event queue
            AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationStatusChanged, eventArgs);

            // Also fire traditional event for backwards compatibility
            _onPushNotificationStatusUpdate?.Invoke(this, eventArgs);
        }
    }
}