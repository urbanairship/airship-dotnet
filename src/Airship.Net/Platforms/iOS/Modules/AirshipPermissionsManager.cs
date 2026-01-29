/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Permissions Manager module.
    /// </summary>
    internal class AirshipPermissionsManager : IAirshipPermissionsManager
    {
        private readonly AirshipModule _module;

        internal AirshipPermissionsManager(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Internal delegate wrapper that bridges IAirshipPermissionDelegate to UAAirshipPermissionDelegate.
        /// </summary>
        private class PermissionDelegateWrapper : UAAirshipPermissionDelegate
        {
            private readonly IAirshipPermissionDelegate _delegate;

            public PermissionDelegateWrapper(IAirshipPermissionDelegate @delegate)
            {
                _delegate = @delegate;
            }

            public override void CheckPermissionStatus(Action<UAPermissionStatus> completionHandler)
            {
                Task.Run(async () =>
                {
                    var status = await _delegate.CheckPermissionStatus();
                    completionHandler(ToUAPermissionStatus(status));
                });
            }

            public override void RequestPermission(Action<UAPermissionStatus> completionHandler)
            {
                Task.Run(async () =>
                {
                    var status = await _delegate.RequestPermission();
                    completionHandler(ToUAPermissionStatus(status));
                });
            }

            private static UAPermissionStatus ToUAPermissionStatus(PermissionStatus status)
            {
                return status switch
                {
                    PermissionStatus.NotDetermined => UAPermissionStatus.NotDetermined,
                    PermissionStatus.Granted => UAPermissionStatus.Granted,
                    PermissionStatus.Denied => UAPermissionStatus.Denied,
                    _ => UAPermissionStatus.NotDetermined
                };
            }
        }

        /// <summary>
        /// Sets a permission delegate for a specific permission.
        /// </summary>
        public void SetDelegate(IAirshipPermissionDelegate? @delegate, Permission permission)
        {
            var uaPermission = ToUAPermission(permission);
            if (@delegate != null)
            {
                var wrapper = new PermissionDelegateWrapper(@delegate);
                UAirship.PermissionsManager.SetDelegate(wrapper, uaPermission);
            }
            else
            {
                UAirship.PermissionsManager.SetDelegate(null, uaPermission);
            }
        }

        /// <summary>
        /// Checks the status of a permission.
        /// </summary>
        public Task<PermissionStatus> CheckPermissionStatus(Permission permission)
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var uaPermission = ToUAPermission(permission);

            UAirship.PermissionsManager.CheckPermissionStatus(uaPermission, (status) =>
            {
                tcs.SetResult(FromUAPermissionStatus(status));
            });

            return tcs.Task;
        }

        /// <summary>
        /// Requests a permission.
        /// </summary>
        public Task<PermissionStatus> RequestPermission(Permission permission)
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var uaPermission = ToUAPermission(permission);

            UAirship.PermissionsManager.RequestPermission(uaPermission, (status) =>
            {
                tcs.SetResult(FromUAPermissionStatus(status));
            });

            return tcs.Task;
        }

        /// <summary>
        /// Requests a permission with option to enable Airship usage on grant.
        /// </summary>
        public Task<PermissionStatus> RequestPermission(Permission permission, bool enableAirshipUsageOnGrant)
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var uaPermission = ToUAPermission(permission);

            UAirship.PermissionsManager.RequestPermission(uaPermission, enableAirshipUsageOnGrant, (status) =>
            {
                tcs.SetResult(FromUAPermissionStatus(status));
            });

            return tcs.Task;
        }

        private static UAPermission ToUAPermission(Permission permission)
        {
            return permission switch
            {
                Permission.DisplayNotifications => UAPermission.DisplayNotifications,
                Permission.Location => UAPermission.Location,
                _ => UAPermission.DisplayNotifications
            };
        }

        private static PermissionStatus FromUAPermissionStatus(UAPermissionStatus status)
        {
            return status switch
            {
                UAPermissionStatus.NotDetermined => PermissionStatus.NotDetermined,
                UAPermissionStatus.Granted => PermissionStatus.Granted,
                UAPermissionStatus.Denied => PermissionStatus.Denied,
                _ => PermissionStatus.NotDetermined
            };
        }
    }
}
