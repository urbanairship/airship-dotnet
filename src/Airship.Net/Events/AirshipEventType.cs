/* Copyright Airship and Contributors */

namespace AirshipDotNet.Events
{
    /// <summary>
    /// Defines the types of events that can be emitted by the Airship SDK.
    /// </summary>
    public enum AirshipEventType
    {
        /// <summary>
        /// Fired when the channel is created.
        /// </summary>
        ChannelCreated,

        /// <summary>
        /// Fired when a push notification is received.
        /// </summary>
        PushReceived,

        /// <summary>
        /// Fired when a background push notification is received.
        /// </summary>
        BackgroundPushReceived,

        /// <summary>
        /// Fired when a push notification response is received.
        /// </summary>
        NotificationResponse,

        /// <summary>
        /// Fired when a background notification response is received.
        /// </summary>
        BackgroundNotificationResponse,

        /// <summary>
        /// Fired when a foreground notification response is received.
        /// </summary>
        ForegroundNotificationResponse,

        /// <summary>
        /// Fired when push notification status changes.
        /// </summary>
        NotificationStatusChanged,

        /// <summary>
        /// Fired when the push token is received.
        /// </summary>
        PushTokenReceived,

        /// <summary>
        /// Fired when a deep link is received.
        /// </summary>
        DeepLinkReceived,

        /// <summary>
        /// Fired when the message center should be displayed.
        /// </summary>
        DisplayMessageCenter,

        /// <summary>
        /// Fired when the message center is updated.
        /// </summary>
        MessageCenterUpdated,

        /// <summary>
        /// Fired when the preference center should be displayed.
        /// </summary>
        DisplayPreferenceCenter,

        /// <summary>
        /// Fired when pending embedded content is updated.
        /// </summary>
        PendingEmbeddedUpdated,

        /// <summary>
        /// iOS only: Fired when authorized notification settings change.
        /// </summary>
        AuthorizedNotificationSettingsChanged
    }
}