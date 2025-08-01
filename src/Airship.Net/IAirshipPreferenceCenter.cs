/* Copyright Airship and Contributors */

using System.Threading.Tasks;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Preference Center interface.
    /// </summary>
    public interface IAirshipPreferenceCenter
    {
        /// <summary>
        /// Opens a preference center.
        /// </summary>
        /// <param name="preferenceCenterId">The preference center ID.</param>
        Task Open(string preferenceCenterId);
    }
}