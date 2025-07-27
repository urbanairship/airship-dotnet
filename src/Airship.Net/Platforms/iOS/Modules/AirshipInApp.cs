/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship In-App Automation module.
    /// </summary>
    internal class AirshipInApp : IAirshipInApp
    {
        private readonly AirshipModule _module;

        internal AirshipInApp(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets whether in-app automation is paused.
        /// </summary>
        public Task<bool> IsPausedAsync()
        {
            return Task.FromResult(UAirship.InAppAutomation.IsPaused);
        }

        /// <summary>
        /// Sets whether in-app automation is paused.
        /// </summary>
        /// <param name="paused">True to pause, false to resume.</param>
        public Task SetPausedAsync(bool paused)
        {
            return Task.Run(() =>
            {
                UAirship.InAppAutomation.IsPaused = paused;
            });
        }

        /// <summary>
        /// Gets the display interval for in-app messages.
        /// </summary>
        /// <returns>The display interval.</returns>
        public Task<TimeSpan> GetDisplayIntervalAsync()
        {
            return Task.FromResult(TimeSpan.FromSeconds(UAirship.InAppAutomation.DisplayInterval));
        }

        /// <summary>
        /// Sets the display interval for in-app messages.
        /// </summary>
        /// <param name="interval">The display interval.</param>
        public Task SetDisplayIntervalAsync(TimeSpan interval)
        {
            return Task.Run(() =>
            {
                UAirship.InAppAutomation.DisplayInterval = interval.TotalSeconds;
            });
        }

        // Additional in-app automation methods would be added here as the SDK bindings are updated
        // to expose more functionality from the native SDK.
    }
}