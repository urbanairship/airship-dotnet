/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet.Events
{
    /// <summary>
    /// Defines the types of events that can be emitted by the Airship SDK.
    /// </summary>
    /// <remarks>
    /// Values are pinned explicitly so that adding or deprecating a member can never
    /// renumber the existing ones. Do not reuse or reorder the values below.
    /// </remarks>
    public enum AirshipEventType
    {
        /// <summary>
        /// Fired when the channel is created.
        /// </summary>
        ChannelCreated = 0,

        /// <summary>
        /// Fired when a push notification is received.
        /// </summary>
        PushReceived = 1,

        /// <summary>
        /// Fired when the user interacts with a push notification.
        /// </summary>
        NotificationResponse = 2,

        /// <summary>
        /// Fired when push notification status changes.
        /// </summary>
        NotificationStatusChanged = 3,

        /// <summary>
        /// Fired when the push token is received.
        /// </summary>
        PushTokenReceived = 4,

        /// <summary>
        /// Fired when a deep link is received.
        /// </summary>
        DeepLinkReceived = 5,

        /// <summary>
        /// Fired when the message center should be displayed.
        /// </summary>
        DisplayMessageCenter = 6,

        /// <summary>
        /// Fired when the message center is updated.
        /// </summary>
        MessageCenterUpdated = 7,

        /// <summary>
        /// Fired when the preference center should be displayed.
        /// </summary>
        DisplayPreferenceCenter = 8,

        /// <summary>
        /// Fired when pending embedded content is updated.
        /// </summary>
        PendingEmbeddedUpdated = 9,

        /// <summary>
        /// iOS only: Fired when authorized notification settings change.
        /// </summary>
        AuthorizedNotificationSettingsChanged = 10,

        /// <summary>
        /// Fired when a background push notification is received.
        /// </summary>
        [Obsolete("Use PushReceived and check PushReceivedEventArgs.IsBackground instead. This member will be removed in a future major release.")]
        BackgroundPushReceived = 11,

        /// <summary>
        /// Fired when a background notification response is received.
        /// </summary>
        [Obsolete("Use NotificationResponse and check NotificationResponseEventArgs.IsForeground instead. This member will be removed in a future major release.")]
        BackgroundNotificationResponse = 12,

        /// <summary>
        /// Fired when a foreground notification response is received.
        /// </summary>
        [Obsolete("Use NotificationResponse and check NotificationResponseEventArgs.IsForeground instead. This member will be removed in a future major release.")]
        ForegroundNotificationResponse = 13
    }
}
