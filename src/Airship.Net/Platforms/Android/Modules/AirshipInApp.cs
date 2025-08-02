/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using UrbanAirship;
using UrbanAirship.Automation;

namespace AirshipDotNet.Platforms.Android.Modules
{
    /// <summary>
    /// Android implementation of Airship InApp module.
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
        /// <returns>True if paused, false otherwise.</returns>
        public Task<bool> IsPaused()
        {
            return Task.FromResult(InAppAutomation.Shared().Paused);
        }

        /// <summary>
        /// Sets whether in-app automation is paused.
        /// </summary>
        /// <param name="paused">True to pause, false to resume.</param>
        public Task SetPaused(bool paused)
        {
            InAppAutomation.Shared().Paused = paused;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the display interval for in-app messages.
        /// </summary>
        /// <returns>The display interval.</returns>
        public Task<TimeSpan> GetDisplayInterval()
        {
            return Task.FromResult(TimeSpan.FromMilliseconds(InAppAutomation.Shared().InAppMessaging!.DisplayInterval));
        }

        /// <summary>
        /// Sets the display interval for in-app messages.
        /// </summary>
        /// <param name="interval">The display interval.</param>
        public Task SetDisplayInterval(TimeSpan interval)
        {
            InAppAutomation.Shared().InAppMessaging!.DisplayInterval = (long)interval.TotalMilliseconds;
            return Task.CompletedTask;
        }
    }
}