/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet;
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
        public Task<bool> IsPaused()
        {
            return Task.FromResult(UAirship.InAppAutomation.IsPaused);
        }

        /// <summary>
        /// Sets whether in-app automation is paused.
        /// </summary>
        /// <param name="paused">True to pause, false to resume.</param>
        public Task SetPaused(bool paused)
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() => UAirship.InAppAutomation.IsPaused = paused);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the display interval for in-app messages.
        /// </summary>
        /// <returns>The display interval.</returns>
        public Task<TimeSpan> GetDisplayInterval()
        {
            return Task.FromResult(TimeSpan.FromSeconds(UAirship.InAppAutomation.DisplayInterval));
        }

        /// <summary>
        /// Sets the display interval for in-app messages.
        /// </summary>
        /// <param name="interval">The display interval.</param>
        public Task SetDisplayInterval(TimeSpan interval)
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() => UAirship.InAppAutomation.DisplayInterval = interval.TotalSeconds);
            return Task.CompletedTask;
        }

        public bool IsEmbeddedAvailable(string embeddedId)
        {
            foreach (var info in AirshipDotNet.Airship.Instance.PendingEmbedded)
                if (info.EmbeddedId == embeddedId) return true;
            return false;
        }

        private readonly object embeddedEventLock = new object();
        private EventHandler<EmbeddedInfoUpdatedEventArgs>? embeddedInfoUpdated;
        private EventHandler<EmbeddedInfoUpdatedEventArgs>? airshipEmbeddedHandler;

        public event EventHandler<EmbeddedInfoUpdatedEventArgs> EmbeddedInfoUpdated
        {
            add
            {
                lock (embeddedEventLock)
                {
                    embeddedInfoUpdated += value;
                    if (airshipEmbeddedHandler == null)
                    {
                        airshipEmbeddedHandler = (s, e) => embeddedInfoUpdated?.Invoke(this, e);
                        AirshipDotNet.Airship.Instance.OnEmbeddedInfoUpdated += airshipEmbeddedHandler;
                    }
                }
            }
            remove
            {
                lock (embeddedEventLock)
                {
                    embeddedInfoUpdated -= value;
                    if (embeddedInfoUpdated == null && airshipEmbeddedHandler != null)
                    {
                        AirshipDotNet.Airship.Instance.OnEmbeddedInfoUpdated -= airshipEmbeddedHandler;
                        airshipEmbeddedHandler = null;
                    }
                }
            }
        }
    }
}