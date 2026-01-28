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
            // TODO: The AirshipObjectiveC xcframework doesn't expose IsOptedIn property directly.
            // This would require checking both UserPushNotificationsEnabled AND system authorization status.
            // For now, returning UserPushNotificationsEnabled as a partial implementation.
            // Full implementation would need: UAirship.Push.AuthorizationStatus != Denied
            return Task.FromResult(UAirship.Push.UserPushNotificationsEnabled);
        }

        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        /// <returns>The current push notification status.</returns>
        public Task<PushNotificationStatus> GetPushNotificationStatus()
        {
            var authStatus = UAirship.Push.AuthorizationStatus;
            var isAuthorized = authStatus != UNAuthorizationStatus.Denied &&
                               authStatus != UNAuthorizationStatus.NotDetermined;

            var permissionStatus = authStatus switch
            {
                UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
                UNAuthorizationStatus.Provisional => PermissionStatus.Granted,
                UNAuthorizationStatus.Ephemeral => PermissionStatus.Granted,
                UNAuthorizationStatus.Denied => PermissionStatus.Denied,
                _ => PermissionStatus.NotDetermined
            };

            var status = new PushNotificationStatus
            {
                IsUserNotificationsEnabled = UAirship.Push.UserPushNotificationsEnabled,
                AreNotificationsAllowed = isAuthorized,
                IsPushPrivacyFeatureEnabled = true,
                IsPushTokenRegistered = !string.IsNullOrEmpty(UAirship.Push.DeviceToken),
                IsUserOptedIn = UAirship.Push.UserPushNotificationsEnabled,
                IsOptIn = UAirship.Push.UserPushNotificationsEnabled && isAuthorized,
                NotificationPermissionStatus = permissionStatus
            };
            return Task.FromResult(status);
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