/* Copyright Airship and Contributors */

using System.Threading.Tasks;
using AirshipDotNet.Analytics;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Analytics interface.
    /// </summary>
    public interface IAirshipAnalytics
    {
        /// <summary>
        /// Tracks a custom event.
        /// </summary>
        /// <param name="customEvent">The custom event to track.</param>
        Task TrackEvent(CustomEvent customEvent);

        /// <summary>
        /// Tracks a screen view.
        /// </summary>
        /// <param name="screen">The screen name.</param>
        Task TrackScreen(string screen);

        /// <summary>
        /// Associates an identifier.
        /// </summary>
        /// <param name="key">The identifier key.</param>
        /// <param name="identifier">The identifier value.</param>
        Task AssociateIdentifier(string key, string identifier);
    }
}