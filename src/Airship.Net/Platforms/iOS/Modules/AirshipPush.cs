/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Foundation;
using Airship;
using UserNotifications;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Push module.
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
            get => UAirship.Push.UserPushNotificationsEnabled;
            set => UAirship.Push.UserPushNotificationsEnabled = value;
        }

        /// <summary>
        /// Checks if user notifications are enabled.
        /// </summary>
        /// <returns>True if user notifications are enabled, false otherwise.</returns>
        public Task<bool> IsUserNotificationsEnabled()
        {
            return Task.FromResult(UAirship.Push.UserPushNotificationsEnabled);
        }

        /// <summary>
        /// Enables or disables user notifications.
        /// </summary>
        /// <param name="enabled">True to enable, false to disable.</param>
        public Task SetUserNotificationsEnabled(bool enabled)
        {
            return Task.Run(() =>
            {
                UAirship.Push.UserPushNotificationsEnabled = enabled;
            });
        }

        /// <summary>
        /// Checks if notifications are opted in.
        /// </summary>
        /// <returns>True if opted in, false otherwise.</returns>
        public Task<bool> IsOptedIn()
        {
            var tcs = new TaskCompletionSource<bool>();

            UAirship.Push.GetNotificationStatus((nativeStatus) =>
            {
                tcs.SetResult(nativeStatus.IsOptedIn);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        /// <returns>The current push notification status.</returns>
        public Task<PushNotificationStatus> GetPushNotificationStatus()
        {
            var tcs = new TaskCompletionSource<PushNotificationStatus>();

            UAirship.Push.GetNotificationStatus((nativeStatus) =>
            {
                var permissionStatus = nativeStatus.NotificationPermissionStatus switch
                {
                    UAPermissionStatus.Granted => PermissionStatus.Granted,
                    UAPermissionStatus.Denied => PermissionStatus.Denied,
                    _ => PermissionStatus.NotDetermined
                };

                var status = new PushNotificationStatus
                {
                    IsUserNotificationsEnabled = nativeStatus.IsUserNotificationsEnabled,
                    AreNotificationsAllowed = nativeStatus.AreNotificationsAllowed,
                    IsPushPrivacyFeatureEnabled = nativeStatus.IsPushPrivacyFeatureEnabled,
                    IsPushTokenRegistered = nativeStatus.IsPushTokenRegistered,
                    IsUserOptedIn = nativeStatus.IsUserOptedIn,
                    IsOptIn = nativeStatus.IsOptedIn,
                    NotificationPermissionStatus = permissionStatus
                };
                tcs.SetResult(status);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Resets the badge count.
        /// </summary>
        /// <returns>A task that completes when the badge is reset.</returns>
        public Task ResetBadge()
        {
            var tcs = new TaskCompletionSource<bool>();

            AWAirshipWrapper.ResetBadgeWithCompletion((error) =>
            {
                if (error != null)
                {
                    tcs.SetException(new Exception($"Failed to reset badge: {error.LocalizedDescription}"));
                }
                else
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Enables user notifications with optional fallback behavior.
        /// </summary>
        /// <param name="args">Optional arguments for enabling notifications.</param>
        /// <returns>True if notifications were enabled, false otherwise.</returns>
        public Task<bool> EnableUserNotifications(EnableUserPushNotificationsArgs? args = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            UAirship.Push.EnableUserPushNotificationsWithCompletionHandler((success) =>
            {
                tcs.SetResult(success);
            });

            return tcs.Task;
        }
    }
}