/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using UrbanAirship;
using UrbanAirship.Push;

namespace AirshipDotNet.Platforms.Android.Modules
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

            // Derive permission status from available information
            PermissionStatus permissionStatus;
            if (nativeStatus.AreNotificationsAllowed)
            {
                permissionStatus = PermissionStatus.Granted;
            }
            else if (nativeStatus.IsUserNotificationsEnabled)
            {
                // User enabled notifications but system doesn't allow - permission was denied
                permissionStatus = PermissionStatus.Denied;
            }
            else
            {
                permissionStatus = PermissionStatus.NotDetermined;
            }

            var status = new PushNotificationStatus
            {
                IsUserNotificationsEnabled = nativeStatus.IsUserNotificationsEnabled,
                AreNotificationsAllowed = nativeStatus.AreNotificationsAllowed,
                IsPushPrivacyFeatureEnabled = nativeStatus.IsPushPrivacyFeatureEnabled,
                IsPushTokenRegistered = nativeStatus.IsPushTokenRegistered,
                IsUserOptedIn = nativeStatus.IsUserOptedIn,
                IsOptIn = nativeStatus.IsOptIn,
                NotificationPermissionStatus = permissionStatus
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
        /// Enables user notifications and prompts for permission if needed.
        /// </summary>
        /// <param name="args">Optional arguments for enabling user notifications.</param>
        /// <returns>True if notifications were enabled successfully, false otherwise.</returns>
        public Task<bool> EnableUserNotifications(EnableUserPushNotificationsArgs? args = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            Com.Urbanairship.Permission.PermissionPromptFallback fallback = args?.Fallback switch
            {
                PromptPermissionFallback.SystemSettings => Com.Urbanairship.Permission.PermissionPromptFallback.SystemSettings.Instance,
                _ => Com.Urbanairship.Permission.PermissionPromptFallback.None.Instance
            };

            _module.UAirship.PushManager.EnableUserNotifications(fallback, new EnableUserNotificationsCallback((result) =>
            {
                tcs.SetResult(result);
            }));

            return tcs.Task;
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

        internal class EnableUserNotificationsCallback : Java.Lang.Object, AndroidX.Core.Util.IConsumer
        {
            private readonly Action<bool> _callback;

            public EnableUserNotificationsCallback(Action<bool> callback)
            {
                _callback = callback;
            }

            public void Accept(Java.Lang.Object? result)
            {
                var boolResult = result as Java.Lang.Boolean;
                _callback?.Invoke(boolResult?.BooleanValue() ?? false);
            }
        }
    }
}