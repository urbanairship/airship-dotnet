/* Copyright Airship and Contributors */

namespace AirshipDotNet
{
    /// <summary>
    /// Permission status for notifications.
    /// </summary>
    public enum PermissionStatus
    {
        /// <summary>
        /// Permission has not been requested yet.
        /// </summary>
        NotDetermined,

        /// <summary>
        /// Permission has been denied.
        /// </summary>
        Denied,

        /// <summary>
        /// Permission has been granted.
        /// </summary>
        Granted
    }

    /// <summary>
    /// Fallback behavior when prompting for notification permission.
    /// </summary>
    public enum PromptPermissionFallback
    {
        /// <summary>
        /// No fallback action.
        /// </summary>
        None,

        /// <summary>
        /// Open system settings if permission was previously denied.
        /// </summary>
        SystemSettings
    }

    /// <summary>
    /// Arguments for enabling user push notifications.
    /// </summary>
    public class EnableUserPushNotificationsArgs
    {
        /// <summary>
        /// Gets the fallback behavior when permission is denied.
        /// </summary>
        public PromptPermissionFallback? Fallback { get; }

        /// <summary>
        /// Initializes a new instance of EnableUserPushNotificationsArgs.
        /// </summary>
        /// <param name="fallback">Optional fallback behavior.</param>
        public EnableUserPushNotificationsArgs(PromptPermissionFallback? fallback = null)
        {
            Fallback = fallback;
        }
    }

    /// <summary>
    /// Push notification status information.
    /// </summary>
    public class PushNotificationStatus
    {
        /// <summary>
        /// Gets or sets whether user notifications are enabled.
        /// </summary>
        public bool IsUserNotificationsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether notifications are allowed.
        /// </summary>
        public bool AreNotificationsAllowed { get; set; }

        /// <summary>
        /// Gets or sets whether the push privacy feature is enabled.
        /// </summary>
        public bool IsPushPrivacyFeatureEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the push token is registered.
        /// </summary>
        public bool IsPushTokenRegistered { get; set; }

        /// <summary>
        /// Gets or sets whether the user has opted in.
        /// </summary>
        public bool IsUserOptedIn { get; set; }

        /// <summary>
        /// Gets or sets whether the system is opted in.
        /// </summary>
        public bool IsOptIn { get; set; }

        /// <summary>
        /// Gets or sets the notification permission status.
        /// </summary>
        public PermissionStatus NotificationPermissionStatus { get; set; } = PermissionStatus.NotDetermined;
    }
}