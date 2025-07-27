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
    }
}