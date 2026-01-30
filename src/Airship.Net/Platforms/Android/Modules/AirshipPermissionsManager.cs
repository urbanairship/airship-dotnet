/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Urbanairship.Permission;
using Java.Interop;
using UrbanAirship;
using AndroidPermission = Com.Urbanairship.Permission.Permission;
using AndroidPermissionStatus = Com.Urbanairship.Permission.PermissionStatus;

namespace AirshipDotNet.Platforms.Android.Modules
{
    /// <summary>
    /// Android implementation of Airship Permissions Manager module.
    /// </summary>
    internal class AirshipPermissionsManager : IAirshipPermissionsManager
    {
        private readonly AirshipModule _module;

        internal AirshipPermissionsManager(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Internal delegate wrapper that bridges IAirshipPermissionDelegate to IPermissionDelegate.
        /// </summary>
        private class PermissionDelegateWrapper : Java.Lang.Object, IPermissionDelegate
        {
            private readonly IAirshipPermissionDelegate _delegate;

            public PermissionDelegateWrapper(IAirshipPermissionDelegate @delegate)
            {
                _delegate = @delegate;
            }

            public void CheckPermissionStatus(global::Android.Content.Context context, global::AndroidX.Core.Util.IConsumer callback)
            {
                Task.Run(async () =>
                {
                    var status = await _delegate.CheckPermissionStatus();
                    var androidStatus = ToAndroidPermissionStatus(status);
                    callback.Accept(androidStatus);
                });
            }

            public void RequestPermission(global::Android.Content.Context context, global::AndroidX.Core.Util.IConsumer callback)
            {
                Task.Run(async () =>
                {
                    var status = await _delegate.RequestPermission();
                    var result = CreatePermissionRequestResult(status);
                    callback.Accept(result);
                });
            }

            private static AndroidPermissionStatus ToAndroidPermissionStatus(AirshipDotNet.PermissionStatus status)
            {
                return status switch
                {
                    AirshipDotNet.PermissionStatus.NotDetermined => AndroidPermissionStatus.NotDetermined!,
                    AirshipDotNet.PermissionStatus.Granted => AndroidPermissionStatus.Granted!,
                    AirshipDotNet.PermissionStatus.Denied => AndroidPermissionStatus.Denied!,
                    _ => AndroidPermissionStatus.NotDetermined!
                };
            }

            /// <summary>
            /// Creates a PermissionRequestResult by calling the Kotlin companion object methods via JNI.
            /// </summary>
            private static PermissionRequestResult CreatePermissionRequestResult(AirshipDotNet.PermissionStatus status)
            {
                // Get the Companion object from the static field
                var resultClass = JNIEnv.FindClass("com/urbanairship/permission/PermissionRequestResult");
                var companionFieldId = JNIEnv.GetStaticFieldID(resultClass, "Companion", "Lcom/urbanairship/permission/PermissionRequestResult$Companion;");
                var companionHandle = JNIEnv.GetStaticObjectField(resultClass, companionFieldId);

                try
                {
                    var companion = Java.Lang.Object.GetObject<PermissionRequestResult.Companion>(companionHandle, JniHandleOwnership.TransferLocalRef);
                    if (companion == null)
                    {
                        throw new InvalidOperationException("Failed to get PermissionRequestResult.Companion");
                    }

                    return status switch
                    {
                        AirshipDotNet.PermissionStatus.Granted => companion.Granted(),
                        AirshipDotNet.PermissionStatus.Denied => companion.Denied(false),
                        _ => companion.NotDetermined()
                    };
                }
                finally
                {
                    JNIEnv.DeleteLocalRef(resultClass);
                }
            }
        }

        /// <summary>
        /// Sets a permission delegate for a specific permission.
        /// </summary>
        public void SetDelegate(IAirshipPermissionDelegate? @delegate, Permission permission)
        {
            var androidPermission = ToAndroidPermission(permission);
            if (@delegate != null)
            {
                var wrapper = new PermissionDelegateWrapper(@delegate);
                UAirship.Shared().PermissionsManager.SetPermissionDelegate(androidPermission, wrapper);
            }
            else
            {
                UAirship.Shared().PermissionsManager.SetPermissionDelegate(androidPermission, null);
            }
        }

        /// <summary>
        /// Checks the status of a permission.
        /// </summary>
        public Task<PermissionStatus> CheckPermissionStatus(Permission permission)
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var androidPermission = ToAndroidPermission(permission);

            var pendingResult = UAirship.Shared().PermissionsManager.CheckPermissionStatusPendingResult(androidPermission);
            pendingResult.AddResultCallback(new PermissionStatusCallback(tcs));

            return tcs.Task;
        }

        /// <summary>
        /// Requests a permission.
        /// </summary>
        public Task<PermissionStatus> RequestPermission(Permission permission)
        {
            return RequestPermission(permission, false);
        }

        /// <summary>
        /// Requests a permission with option to enable Airship usage on grant.
        /// </summary>
        public Task<PermissionStatus> RequestPermission(Permission permission, bool enableAirshipUsageOnGrant)
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var androidPermission = ToAndroidPermission(permission);

            var pendingResult = UAirship.Shared().PermissionsManager.RequestPermissionPendingResult(androidPermission, enableAirshipUsageOnGrant);
            pendingResult.AddResultCallback(new PermissionRequestResultCallback(tcs));

            return tcs.Task;
        }

        /// <summary>
        /// Callback for permission status checks.
        /// </summary>
        private class PermissionStatusCallback : Java.Lang.Object, UrbanAirship.IResultCallback
        {
            private readonly TaskCompletionSource<PermissionStatus> _tcs;

            public PermissionStatusCallback(TaskCompletionSource<PermissionStatus> tcs)
            {
                _tcs = tcs;
            }

            public void OnResult(Java.Lang.Object? result)
            {
                var status = result as Com.Urbanairship.Permission.PermissionStatus;
                _tcs.SetResult(FromAndroidPermissionStatus(status));
            }
        }

        /// <summary>
        /// Callback for permission request results.
        /// </summary>
        private class PermissionRequestResultCallback : Java.Lang.Object, UrbanAirship.IResultCallback
        {
            private readonly TaskCompletionSource<PermissionStatus> _tcs;

            public PermissionRequestResultCallback(TaskCompletionSource<PermissionStatus> tcs)
            {
                _tcs = tcs;
            }

            public void OnResult(Java.Lang.Object? result)
            {
                var requestResult = result as PermissionRequestResult;
                var status = requestResult?.PermissionStatus;
                _tcs.SetResult(FromAndroidPermissionStatus(status));
            }
        }

        private static Com.Urbanairship.Permission.Permission ToAndroidPermission(Permission permission)
        {
            return permission switch
            {
                Permission.DisplayNotifications => Com.Urbanairship.Permission.Permission.DisplayNotifications!,
                Permission.Location => Com.Urbanairship.Permission.Permission.Location!,
                _ => Com.Urbanairship.Permission.Permission.DisplayNotifications!
            };
        }

        private static PermissionStatus FromAndroidPermissionStatus(Com.Urbanairship.Permission.PermissionStatus? status)
        {
            if (status == null)
            {
                return PermissionStatus.NotDetermined;
            }

            // Compare by name since these are Java enums
            if (status.Name() == Com.Urbanairship.Permission.PermissionStatus.Granted?.Name())
            {
                return PermissionStatus.Granted;
            }
            else if (status.Name() == Com.Urbanairship.Permission.PermissionStatus.Denied?.Name())
            {
                return PermissionStatus.Denied;
            }
            else
            {
                return PermissionStatus.NotDetermined;
            }
        }
    }
}
