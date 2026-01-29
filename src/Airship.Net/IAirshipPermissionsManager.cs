/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;

namespace AirshipDotNet
{
    /// <summary>
    /// Delegate for handling permission requests.
    /// </summary>
    public interface IAirshipPermissionDelegate
    {
        /// <summary>
        /// Called when a permission status needs to be checked.
        /// </summary>
        /// <returns>The current permission status.</returns>
        Task<PermissionStatus> CheckPermissionStatus();

        /// <summary>
        /// Called when a permission should be requested.
        /// </summary>
        /// <returns>The permission status after the request.</returns>
        Task<PermissionStatus> RequestPermission();
    }

    /// <summary>
    /// Airship Permissions Manager interface.
    /// </summary>
    public interface IAirshipPermissionsManager
    {
        /// <summary>
        /// Sets a permission delegate for a specific permission.
        /// </summary>
        /// <param name="delegate">The delegate to handle permission requests.</param>
        /// <param name="permission">The permission type.</param>
        void SetDelegate(IAirshipPermissionDelegate? @delegate, Permission permission);

        /// <summary>
        /// Checks the status of a permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The current status of the permission.</returns>
        Task<PermissionStatus> CheckPermissionStatus(Permission permission);

        /// <summary>
        /// Requests a permission.
        /// </summary>
        /// <param name="permission">The permission to request.</param>
        /// <returns>The status after the request.</returns>
        Task<PermissionStatus> RequestPermission(Permission permission);

        /// <summary>
        /// Requests a permission with option to enable Airship usage on grant.
        /// </summary>
        /// <param name="permission">The permission to request.</param>
        /// <param name="enableAirshipUsageOnGrant">If true, enables Airship features that need this permission when granted.</param>
        /// <returns>The status after the request.</returns>
        Task<PermissionStatus> RequestPermission(Permission permission, bool enableAirshipUsageOnGrant);
    }
}
