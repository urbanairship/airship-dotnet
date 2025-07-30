/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using UrbanAirship;
using UrbanAirship.Push;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship Push module.
    /// </summary>
    internal class AirshipPush : IAirshipPush
    {
        private readonly AirshipModule _module;

        internal AirshipPush(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets or sets whether user notifications are enabled.
        /// </summary>
        public bool UserNotificationsEnabled
        {
            get => _module.UAirship.PushManager.UserNotificationsEnabled;
            set => _module.UAirship.PushManager.UserNotificationsEnabled = value;
        }

        /// <summary>
        /// Checks if notifications are opted in.
        /// </summary>
        /// <returns>True if opted in, false otherwise.</returns>
        public Task<bool> IsOptedIn()
        {
            var status = _module.UAirship.PushManager.PushNotificationStatus;
            return Task.FromResult(status.IsOptIn);
        }

        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        /// <returns>The current push notification status.</returns>
        public Task<PushNotificationStatus> GetPushNotificationStatus()
        {
            var nativeStatus = _module.UAirship.PushManager.PushNotificationStatus;

            var status = new PushNotificationStatus
            {
                IsUserNotificationsEnabled = nativeStatus.IsUserNotificationsEnabled,
                AreNotificationsAllowed = nativeStatus.AreNotificationsAllowed,
                IsPushPrivacyFeatureEnabled = nativeStatus.IsPushPrivacyFeatureEnabled,
                IsPushTokenRegistered = nativeStatus.IsPushTokenRegistered,
                IsUserOptedIn = nativeStatus.IsUserOptedIn,
                IsOptIn = nativeStatus.IsOptIn
            };

            return Task.FromResult(status);
        }

        /// <summary>
        /// Checks if user notifications are enabled.
        /// </summary>
        /// <returns>True if user notifications are enabled, false otherwise.</returns>
        public Task<bool> IsUserNotificationsEnabled()
        {
            return Task.FromResult(UserNotificationsEnabled);
        }

        /// <summary>
        /// Enables or disables user notifications.
        /// </summary>
        /// <param name="enabled">True to enable, false to disable.</param>
        public Task SetUserNotificationsEnabled(bool enabled)
        {
            UserNotificationsEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Resets the badge count.
        /// </summary>
        /// <returns>A task that completes when the badge is reset.</returns>
        public Task ResetBadge()
        {
            // Badge management is handled differently on Android
            // Most Android launchers don't support badges like iOS
            return Task.CompletedTask;
        }
    }
}