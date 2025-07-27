/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship In-App Automation interface.
    /// </summary>
    public interface IAirshipInApp
    {
        /// <summary>
        /// Gets whether in-app automation is paused.
        /// </summary>
        /// <returns>True if paused, false otherwise.</returns>
        Task<bool> IsPausedAsync();
        
        /// <summary>
        /// Sets whether in-app automation is paused.
        /// </summary>
        /// <param name="paused">True to pause, false to resume.</param>
        Task SetPausedAsync(bool paused);
        
        /// <summary>
        /// Gets the display interval for in-app messages.
        /// </summary>
        /// <returns>The display interval.</returns>
        Task<TimeSpan> GetDisplayIntervalAsync();
        
        /// <summary>
        /// Sets the display interval for in-app messages.
        /// </summary>
        /// <param name="interval">The display interval.</param>
        Task SetDisplayIntervalAsync(TimeSpan interval);
    }
}