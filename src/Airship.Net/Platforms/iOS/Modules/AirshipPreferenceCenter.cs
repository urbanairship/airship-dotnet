/* Copyright Airship and Contributors */

using System.Threading.Tasks;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship PreferenceCenter module.
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
            UAirship.PreferenceCenter.OpenPreferenceCenter(preferenceCenterId);
            return Task.CompletedTask;
        }
    }
}