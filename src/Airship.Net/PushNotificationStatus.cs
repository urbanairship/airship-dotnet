/* Copyright Airship and Contributors */

namespace AirshipDotNet
{
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

    /// <summary>
    /// Permission status values.
    /// </summary>
    public enum PermissionStatus
    {
        /// <summary>
        /// Permission is granted.
        /// </summary>
        Granted,

        /// <summary>
        /// Permission is denied.
        /// </summary>
        Denied,

        /// <summary>
        /// Permission has not yet been requested.
        /// </summary>
        NotDetermined
    }

    /// <summary>
    /// Fallback behavior when prompting for permission and the permission is
    /// already denied on iOS or is denied silently on Android.
    /// </summary>
    public enum PromptPermissionFallback
    {
        /// <summary>
        /// No fallback action.
        /// </summary>
        None,

        /// <summary>
        /// Take the user to the system settings to enable the permission.
        /// </summary>
        SystemSettings
    }

    /// <summary>
    /// Options for enabling push notifications.
    /// </summary>
    public class EnableUserPushNotificationsArgs
    {
        /// <summary>
        /// Gets the fallback strategy when permission is denied.
        /// </summary>
        public PromptPermissionFallback? Fallback { get; }

        /// <summary>
        /// Creates a new instance of EnableUserPushNotificationsArgs.
        /// </summary>
        /// <param name="fallback">Optional fallback strategy.</param>
        public EnableUserPushNotificationsArgs(PromptPermissionFallback? fallback = null)
        {
            Fallback = fallback;
        }
    }
}