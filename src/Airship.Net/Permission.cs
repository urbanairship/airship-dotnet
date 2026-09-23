/* Copyright Airship and Contributors */

namespace AirshipDotNet
{
    /// <summary>
    /// Airship permissions used with PermissionsManager.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// Permission to display notifications.
        /// </summary>
        DisplayNotifications = 0,

        /// <summary>
        /// Permission to access location.
        /// </summary>
        Location = 1,

        /// <summary>
        /// Permission to track the user across apps and websites (iOS App Tracking Transparency).
        /// </summary>
        AppTrackingTransparency = 2,

        /// <summary>
        /// Permission to access the camera.
        /// </summary>
        Camera = 3,

        /// <summary>
        /// Permission to access the microphone.
        /// </summary>
        Microphone = 4,

        /// <summary>
        /// Permission to access Bluetooth.
        /// </summary>
        Bluetooth = 5,

        /// <summary>
        /// Permission to access the photo library.
        /// </summary>
        PhotoLibrary = 6,

        /// <summary>
        /// Permission to access contacts.
        /// </summary>
        Contacts = 7
    }
}
