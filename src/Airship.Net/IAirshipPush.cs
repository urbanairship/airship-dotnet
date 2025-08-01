/* Copyright Airship and Contributors */

using System.Threading.Tasks;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Push interface.
    /// </summary>
    public interface IAirshipPush
    {
        /// <summary>
        /// Gets or sets whether user notifications are enabled.
        /// </summary>
        bool UserNotificationsEnabled { get; set; }

        /// <summary>
        /// Checks if user notifications are enabled.
        /// </summary>
        /// <returns>True if user notifications are enabled, false otherwise.</returns>
        Task<bool> IsUserNotificationsEnabled();

        /// <summary>
        /// Enables or disables user notifications.
        /// </summary>
        /// <param name="enabled">True to enable, false to disable.</param>
        Task SetUserNotificationsEnabled(bool enabled);

        /// <summary>
        /// Checks if notifications are opted in.
        /// </summary>
        /// <returns>True if opted in, false otherwise.</returns>
        Task<bool> IsOptedIn();

        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        /// <returns>The current push notification status.</returns>
        Task<PushNotificationStatus> GetPushNotificationStatus();

        /// <summary>
        /// Resets the badge count.
        /// </summary>
        /// <returns>A task that completes when the badge is reset.</returns>
        Task ResetBadge();
    }
}