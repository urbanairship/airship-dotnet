/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using UrbanAirship;
using UrbanAirship.Automation;

namespace AirshipDotNet
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
        /// Gets or sets whether in-app automation is paused.
        /// </summary>
        public bool IsPaused
        {
            get => InAppAutomation.Shared().Paused;
            set => InAppAutomation.Shared().Paused = value;
        }

        /// <summary>
        /// Gets or sets the display interval for in-app messages.
        /// </summary>
        public TimeSpan DisplayInterval
        {
            get => TimeSpan.FromMilliseconds(InAppAutomation.Shared().InAppMessaging!.DisplayInterval);
            set => InAppAutomation.Shared().InAppMessaging!.DisplayInterval = (long)value.TotalMilliseconds;
        }

        /// <summary>
        /// Gets whether in-app automation is paused.
        /// </summary>
        /// <returns>True if paused, false otherwise.</returns>
        public Task<bool> IsPausedAsync()
        {
            return Task.FromResult(IsPaused);
        }

        /// <summary>
        /// Sets whether in-app automation is paused.
        /// </summary>
        /// <param name="paused">True to pause, false to resume.</param>
        public Task SetPausedAsync(bool paused)
        {
            IsPaused = paused;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the display interval for in-app messages.
        /// </summary>
        /// <returns>The display interval.</returns>
        public Task<TimeSpan> GetDisplayIntervalAsync()
        {
            return Task.FromResult(DisplayInterval);
        }

        /// <summary>
        /// Sets the display interval for in-app messages.
        /// </summary>
        /// <param name="interval">The display interval.</param>
        public Task SetDisplayIntervalAsync(TimeSpan interval)
        {
            DisplayInterval = interval;
            return Task.CompletedTask;
        }
    }
}