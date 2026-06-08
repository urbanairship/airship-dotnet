/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using UserNotifications;
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

    internal class AirshipPushNotificationDelegate : global::Airship.UAPushNotificationDelegate
    {
        private readonly Action<NSDictionary, bool> onReceived;
        private readonly Action<UNNotificationResponse> onResponse;

        public AirshipPushNotificationDelegate(Action<NSDictionary, bool> onReceived, Action<UNNotificationResponse> onResponse)
        {
            this.onReceived = onReceived;
            this.onResponse = onResponse;
        }

        public override void ReceivedForegroundNotification(NSDictionary userInfo, Action completionHandler)
        {
            onReceived?.Invoke(userInfo, false);
            completionHandler();
        }

        public override void ReceivedBackgroundNotification(NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            onReceived?.Invoke(userInfo, true);
            completionHandler(UIBackgroundFetchResult.NoData);
        }

        public override void ReceivedNotificationResponse(UNNotificationResponse notificationResponse, Action completionHandler)
        {
            onResponse?.Invoke(notificationResponse);
            completionHandler();
        }

        public override void ExtendPresentationOptions(UNNotificationPresentationOptions options, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(options);
        }
    }

    internal class AirshipRegistrationDelegate : global::Airship.UARegistrationDelegate
    {
        private readonly Action<string> onTokenReceived;
        private readonly Action<UAAuthorizedNotificationSettings> onSettingsChanged;

        public AirshipRegistrationDelegate(Action<string> onTokenReceived, Action<UAAuthorizedNotificationSettings> onSettingsChanged)
        {
            this.onTokenReceived = onTokenReceived;
            this.onSettingsChanged = onSettingsChanged;
        }

        public override void NotificationRegistrationFinishedWithAuthorizedSettings(UAAuthorizedNotificationSettings authorizedSettings, NSSet<UNNotificationCategory> categories, UNAuthorizationStatus status) { }

        public override void NotificationRegistrationFinishedWithAuthorizedSettings(UAAuthorizedNotificationSettings authorizedSettings, UNAuthorizationStatus status) { }

        public override void NotificationAuthorizedSettingsDidChange(UAAuthorizedNotificationSettings authorizedSettings)
        {
            onSettingsChanged?.Invoke(authorizedSettings);
        }

        public override void ApnsRegistrationSucceededWithDeviceToken(NSData deviceToken)
        {
            onTokenReceived?.Invoke(HexFromData(deviceToken));
        }

        public override void ApnsRegistrationFailedWithError(NSError error) { }

        private static string HexFromData(NSData data)
        {
            var bytes = data.ToArray();
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    internal class AirshipPreferenceCenterOpenDelegate : global::Airship.UAPreferenceCenterOpenDelegate
    {
        private readonly Action<string> handler;

        public AirshipPreferenceCenterOpenDelegate(Action<string> handler)
        {
            this.handler = handler;
        }

        public override bool OpenPreferenceCenter(string preferenceCenterID)
        {
            handler?.Invoke(preferenceCenterID);
            return true;
        }
    }

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
        private readonly Dictionary<EventHandler<PushReceivedEventArgs>, EventHandler<EventArgs>> _pushReceivedHandlerMap = new();
        private readonly Dictionary<EventHandler<NotificationResponseEventArgs>, EventHandler<EventArgs>> _notificationResponseHandlerMap = new();
        private readonly Dictionary<EventHandler<PushTokenReceivedEventArgs>, EventHandler<EventArgs>> _pushTokenHandlerMap = new();
        private readonly Dictionary<EventHandler<MessageCenterEventArgs>, EventHandler<EventArgs>> _displayMessageCenterHandlerMap = new();
        private readonly Dictionary<EventHandler<EventArgs>, EventHandler<EventArgs>> _messageCenterUpdatedHandlerMap = new();
        private readonly Dictionary<EventHandler<PreferenceCenterEventArgs>, EventHandler<EventArgs>> _displayPreferenceCenterHandlerMap = new();
        private readonly Dictionary<EventHandler<IOSAuthorizedNotificationSettingsEventArgs>, EventHandler<EventArgs>> _authorizedSettingsHandlerMap = new();

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

        // Strong references for delegates Airship binds weakly
        private AirshipDeepLinkDelegate? _deepLinkDelegate;
        private AirshipPushNotificationDelegate? _pushNotificationDelegate;
        private AirshipRegistrationDelegate? _registrationDelegate;
        private AirshipPreferenceCenterOpenDelegate? _preferenceCenterOpenDelegate;
        private AirshipMessageCenterDisplayDelegate? _messageCenterDisplayDelegate;

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
                AirshipEventEmitter.Shared.Emit(AirshipEventType.ChannelCreated, new ChannelEventArgs(channelID));
            });

            // Message Center inbox updated notification
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)"com.urbanairship.notification.message_list_updated", (_) =>
            {
                AirshipEventEmitter.Shared.Emit(AirshipEventType.MessageCenterUpdated, EventArgs.Empty);
            });

            // Push (foreground/background) and notification response
            _pushNotificationDelegate = new AirshipPushNotificationDelegate(
                onReceived: (userInfo, isBackground) =>
                {
                    AirshipEventEmitter.Shared.Emit(AirshipEventType.PushReceived, BuildPushReceivedEventArgs(userInfo, isBackground));
                },
                onResponse: (response) =>
                {
                    var userInfo = response.Notification.Request.Content.UserInfo;
                    var push = BuildPushReceivedEventArgs(userInfo, isBackground: false);
                    var actionId = response.ActionIdentifier == "com.apple.UNNotificationDefaultActionIdentifier" ? null : response.ActionIdentifier;
                    AirshipEventEmitter.Shared.Emit(AirshipEventType.NotificationResponse, new NotificationResponseEventArgs(push, actionId, isForeground: true));
                });
            UAirship.Push.PushNotificationDelegate = _pushNotificationDelegate;

            // Push token + authorized settings changes
            _registrationDelegate = new AirshipRegistrationDelegate(
                onTokenReceived: (token) =>
                {
                    AirshipEventEmitter.Shared.Emit(AirshipEventType.PushTokenReceived, new PushTokenReceivedEventArgs(token));
                },
                onSettingsChanged: (settings) =>
                {
                    // UAAuthorizedNotificationSettings is a Swift OptionSet wrapper whose
                    // rawValue is not exposed via the current ObjC binding (no @objc accessor,
                    // not KVC-compliant). Emit 0 until the binding surfaces the raw bitmask.
                    AirshipEventEmitter.Shared.Emit(AirshipEventType.AuthorizedNotificationSettingsChanged, new IOSAuthorizedNotificationSettingsEventArgs(0));
                });
            UAirship.Push.RegistrationDelegate = _registrationDelegate;

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

                if (_deepLinkDelegate == null)
                {
                    _deepLinkDelegate = new AirshipDeepLinkDelegate((deepLink) =>
                    {
                        AirshipEventEmitter.Shared.Emit(AirshipEventType.DeepLinkReceived, new DeepLinkEventArgs(deepLink));
                    });
                    UAirship.DeepLinkDelegate = _deepLinkDelegate;
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
                    _deepLinkDelegate = null;
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
                        UAirship.MessageCenter.DisplayDelegate = _messageCenterDisplayDelegate;
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
                    UAirship.MessageCenter.DisplayDelegate = null;
                    _messageCenterDisplayDelegate = null;
                }
            }
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
                        _preferenceCenterOpenDelegate = new AirshipPreferenceCenterOpenDelegate((preferenceCenterID) =>
                        {
                            AirshipEventEmitter.Shared.Emit(AirshipEventType.DisplayPreferenceCenter, new PreferenceCenterEventArgs(preferenceCenterID));
                        });
                        UAirship.PreferenceCenter.OpenDelegate = _preferenceCenterOpenDelegate;
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
                    UAirship.PreferenceCenter.OpenDelegate = null;
                    _preferenceCenterOpenDelegate = null;
                }
            }
        }

        /// <summary>
        /// Add/remove the iOS authorized notification settings change listener.
        /// </summary>
        public event EventHandler<IOSAuthorizedNotificationSettingsEventArgs>? OnAuthorizedNotificationSettingsChanged
        {
            add
            {
                if (value != null)
                {
                    EventHandler<EventArgs> wrapper = (sender, args) =>
                        value(this, (args as IOSAuthorizedNotificationSettingsEventArgs)!);

                    _authorizedSettingsHandlerMap[value] = wrapper;
                    AirshipEventEmitter.Shared.AddListener(AirshipEventType.AuthorizedNotificationSettingsChanged, wrapper);
                }
            }
            remove
            {
                if (value != null && _authorizedSettingsHandlerMap.TryGetValue(value, out var wrapper))
                {
                    AirshipEventEmitter.Shared.RemoveListener(AirshipEventType.AuthorizedNotificationSettingsChanged, wrapper);
                    _authorizedSettingsHandlerMap.Remove(value);
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

        private static PushReceivedEventArgs BuildPushReceivedEventArgs(NSDictionary userInfo, bool isBackground)
        {
            var payload = ToManagedDictionary(userInfo);
            string? alert = null;
            string? title = null;

            if (payload.TryGetValue("aps", out var apsObj) && apsObj is IDictionary<string, object?> aps)
            {
                if (aps.TryGetValue("alert", out var alertObj))
                {
                    if (alertObj is IDictionary<string, object?> alertDict)
                    {
                        if (alertDict.TryGetValue("body", out var body)) alert = body?.ToString();
                        if (alertDict.TryGetValue("title", out var t)) title = t?.ToString();
                    }
                    else
                    {
                        alert = alertObj?.ToString();
                    }
                }
            }

            return new PushReceivedEventArgs(payload, alert, title, isBackground);
        }

        private static IDictionary<string, object?> ToManagedDictionary(NSDictionary nsDict)
        {
            var dict = new Dictionary<string, object?>();
            if (nsDict == null) return dict;

            foreach (var key in nsDict.Keys)
            {
                var keyString = key.ToString();
                if (keyString == null) continue;
                dict[keyString] = ConvertNSObject(nsDict[key]);
            }
            return dict;
        }

        private static object? ConvertNSObject(NSObject? value)
        {
            return value switch
            {
                null => null,
                NSString s => (string)s,
                NSNumber n => n.DoubleValue,
                NSDictionary d => ToManagedDictionary(d),
                NSArray a => Enumerable.Range(0, (int)a.Count).Select(i => ConvertNSObject(a.GetItem<NSObject>((nuint)i))).ToList(),
                _ => value.ToString()
            };
        }

    }
}
