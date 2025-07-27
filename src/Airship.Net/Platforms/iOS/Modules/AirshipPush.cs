/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Foundation;
using Airship;
using UserNotifications;

namespace AirshipDotNet
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
        public Task<bool> IsUserNotificationsEnabledAsync()
        {
            return Task.FromResult(UAirship.Push.UserPushNotificationsEnabled);
        }

        /// <summary>
        /// Enables or disables user notifications.
        /// </summary>
        /// <param name="enabled">True to enable, false to disable.</param>
        public Task SetUserNotificationsEnabledAsync(bool enabled)
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
        public Task<bool> IsOptedInAsync()
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
        public Task<PushNotificationStatus> GetPushNotificationStatusAsync()
        {
            // TODO: The AirshipObjectiveC xcframework doesn't expose all properties needed for full status.
            // Missing: IsPushPrivacyFeatureEnabled, IsPushTokenRegistered, IsUserOptedIn
            // Available: AuthorizationStatus, AuthorizedNotificationSettings, UserPushNotificationsEnabled
            var isAuthorized = UAirship.Push.AuthorizationStatus != UserNotifications.UNAuthorizationStatus.Denied &&
                               UAirship.Push.AuthorizationStatus != UserNotifications.UNAuthorizationStatus.NotDetermined;

            var status = new PushNotificationStatus
            {
                IsUserNotificationsEnabled = UAirship.Push.UserPushNotificationsEnabled,
                AreNotificationsAllowed = isAuthorized,
                IsPushPrivacyFeatureEnabled = true, // Assuming enabled if push module is available
                IsPushTokenRegistered = !string.IsNullOrEmpty(UAirship.Push.DeviceToken),
                IsUserOptedIn = UAirship.Push.UserPushNotificationsEnabled,
                IsOptIn = UAirship.Push.UserPushNotificationsEnabled && isAuthorized
            };
            return Task.FromResult(status);
        }

        /// <summary>
        /// Resets the badge count.
        /// </summary>
        /// <returns>A task that completes when the badge is reset.</returns>
        public Task ResetBadgeAsync()
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

        // Event handlers will be added in a future phase to maintain existing event patterns
        // For now, users can still use the legacy events through Airship.Instance if needed
    }
}