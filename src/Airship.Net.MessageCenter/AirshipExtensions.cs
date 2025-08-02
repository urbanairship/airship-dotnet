/* Copyright Airship and Contributors */

using AirshipDotNet.MessageCenter;

namespace AirshipDotNet
{
    /// <summary>
    /// Extensions for Airship to provide MessageCenter functionality.
    /// </summary>
    public static class AirshipExtensions
    {
#if IOS || ANDROID
        private static IAirshipMessageCenter? _messageCenter;

        /// <summary>
        /// Gets the MessageCenter instance.
        /// </summary>
        /// <param name="airship">The Airship instance.</param>
        /// <returns>The MessageCenter instance.</returns>
        public static IAirshipMessageCenter MessageCenter(this Airship airship)
        {
            if (_messageCenter == null)
            {
#if IOS
                _messageCenter = new MessageCenter.Platforms.iOS.Modules.AirshipMessageCenter(new AirshipDotNet.Platforms.iOS.AirshipModule());
#elif ANDROID
                _messageCenter = new MessageCenter.Platforms.Android.Modules.AirshipMessageCenter(new AirshipDotNet.Platforms.Android.AirshipModule());
#endif
            }
            return _messageCenter;
        }
#endif
    }
}