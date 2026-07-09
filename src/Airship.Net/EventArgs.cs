/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;

namespace AirshipDotNet
{
    /// <summary>
    /// Event args for channel creation events.
    /// </summary>
    public class ChannelEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the channel ID.
        /// </summary>
        public string ChannelId { get; }
        
        /// <summary>
        /// Initializes a new instance of the ChannelEventArgs class.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        public ChannelEventArgs(string channelId)
        {
            ChannelId = channelId;
        }
    }
    
    /// <summary>
    /// Event args for push notification status update events.
    /// </summary>
    public class PushNotificationStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        public PushNotificationStatus Status { get; }
        
        /// <summary>
        /// Initializes a new instance of the PushNotificationStatusEventArgs class.
        /// </summary>
        /// <param name="status">The push notification status.</param>
        public PushNotificationStatusEventArgs(PushNotificationStatus status)
        {
            Status = status;
        }
    }
    
    /// <summary>
    /// Event args for push notifications received.
    /// </summary>
    public class PushReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the raw notification payload (APNs userInfo on iOS, push extras on Android).
        /// </summary>
        public IDictionary<string, object?> Payload { get; }

        /// <summary>
        /// Gets the alert text, or null if not present.
        /// </summary>
        public string? Alert { get; }

        /// <summary>
        /// Gets the notification title, or null if not present.
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// True when this was a silent/background push that did not post a notification.
        /// </summary>
        public bool IsBackground { get; }

        public PushReceivedEventArgs(IDictionary<string, object?> payload, string? alert, string? title, bool isBackground)
        {
            Payload = payload;
            Alert = alert;
            Title = title;
            IsBackground = isBackground;
        }
    }

    /// <summary>
    /// Event args for the user interacting with a notification.
    /// </summary>
    public class NotificationResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the underlying push payload.
        /// </summary>
        public PushReceivedEventArgs Push { get; }

        /// <summary>
        /// Gets the action identifier the user tapped, or null for the default tap.
        /// </summary>
        public string? ActionId { get; }

        /// <summary>
        /// True if the action ran in the foreground.
        /// </summary>
        public bool IsForeground { get; }

        public NotificationResponseEventArgs(PushReceivedEventArgs push, string? actionId, bool isForeground)
        {
            Push = push;
            ActionId = actionId;
            IsForeground = isForeground;
        }
    }

    /// <summary>
    /// Event args for push token registration.
    /// </summary>
    public class PushTokenReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the push token (APNs device token hex on iOS, FCM registration token on Android).
        /// </summary>
        public string Token { get; }

        public PushTokenReceivedEventArgs(string token)
        {
            Token = token;
        }
    }

    /// <summary>
    /// Event args for preference center open requests.
    /// </summary>
    public class PreferenceCenterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the preference center identifier the user requested.
        /// </summary>
        public string PreferenceCenterId { get; }

        public PreferenceCenterEventArgs(string preferenceCenterId)
        {
            PreferenceCenterId = preferenceCenterId;
        }
    }

    /// <summary>
    /// Event args for iOS authorized notification settings changes.
    /// </summary>
    public class IOSAuthorizedNotificationSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the raw authorized settings bitmask reported by iOS.
        /// </summary>
        public ulong AuthorizedSettings { get; }

        public IOSAuthorizedNotificationSettingsEventArgs(ulong authorizedSettings)
        {
            AuthorizedSettings = authorizedSettings;
        }
    }

    /// <summary>
    /// Event args for deep link events.
    /// </summary>
    public class DeepLinkEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the deep link.
        /// </summary>
        public string DeepLink { get; }

        /// <summary>
        /// Initializes a new instance of the DeepLinkEventArgs class.
        /// </summary>
        /// <param name="deepLink">The deep link.</param>
        public DeepLinkEventArgs(string deepLink)
        {
            DeepLink = deepLink;
        }
    }

    /// <summary>Event args for embedded content availability updates.</summary>
    public class EmbeddedInfoUpdatedEventArgs : EventArgs
    {
        /// <summary>The current set of pending embedded content.</summary>
        public IReadOnlyList<EmbeddedInfo> Pending { get; }

        public EmbeddedInfoUpdatedEventArgs(IReadOnlyList<EmbeddedInfo> pending)
        {
            Pending = pending;
        }
    }
}

namespace AirshipDotNet.MessageCenter
{
    /// <summary>
    /// Event args for message center display events.
    /// </summary>
    public class MessageCenterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message ID, or null if displaying the message center list.
        /// </summary>
        public string? MessageId { get; }

        /// <summary>
        /// Initializes a new instance of the MessageCenterEventArgs class.
        /// </summary>
        /// <param name="messageId">The message ID, or null if displaying the message center list.</param>
        public MessageCenterEventArgs(string? messageId)
        {
            MessageId = messageId;
        }
    }
}