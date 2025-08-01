/* Copyright Airship and Contributors */

using System.Threading.Tasks;
using UrbanAirship.PreferenceCenter;

namespace AirshipDotNet.Platforms.Android.Modules
{
    /// <summary>
    /// Android implementation of Airship PreferenceCenter module.
    /// </summary>
    internal class AirshipPreferenceCenter : IAirshipPreferenceCenter
    {
        private readonly AirshipModule _module;

        internal AirshipPreferenceCenter(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Opens a preference center.
        /// </summary>
        /// <param name="preferenceCenterId">The preference center ID.</param>
        public Task Open(string preferenceCenterId)
        {
            PreferenceCenter.Shared().Open(preferenceCenterId);
            return Task.CompletedTask;
        }
    }
}