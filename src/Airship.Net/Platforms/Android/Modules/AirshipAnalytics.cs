/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using AirshipDotNet.Analytics;
using UrbanAirship;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship Analytics module.
    /// </summary>
    internal class AirshipAnalytics : IAirshipAnalytics
    {
        private readonly AirshipModule _module;

        internal AirshipAnalytics(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Tracks a screen view.
        /// </summary>
        /// <param name="screen">The screen name.</param>
        public Task TrackScreenAsync(string screen)
        {
            _module.UAirship.Analytics.TrackScreen(screen);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks a custom event.
        /// </summary>
        /// <param name="customEvent">The custom event to track.</param>
        public Task TrackEventAsync(CustomEvent customEvent)
        {
            if (customEvent == null || string.IsNullOrEmpty(customEvent.EventName))
            {
                return Task.CompletedTask;
            }

            var builder = new UrbanAirship.Analytics.CustomEvent.Builder(customEvent.EventName);

            if (customEvent.EventValue != null)
            {
                builder.SetEventValue((double)customEvent.EventValue);
            }

            if (!string.IsNullOrEmpty(customEvent.TransactionId))
            {
                builder.SetTransactionId(customEvent.TransactionId);
            }

            if (!string.IsNullOrEmpty(customEvent.InteractionType) && !string.IsNullOrEmpty(customEvent.InteractionId))
            {
                builder.SetInteraction(customEvent.InteractionType, customEvent.InteractionId);
            }

            if (customEvent.PropertyList != null)
            {
                foreach (dynamic property in customEvent.PropertyList)
                {
                    if (string.IsNullOrEmpty(property.Name))
                    {
                        continue;
                    }

                    builder.AddProperty(property.Name, property.Value);
                }
            }

            _module.UAirship.Analytics.AddEvent(builder.Build());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Associates an identifier.
        /// </summary>
        /// <param name="key">The identifier key.</param>
        /// <param name="identifier">The identifier value.</param>
        public Task AssociateIdentifierAsync(string key, string? identifier)
        {
            if (identifier == null)
            {
                _module.UAirship.Analytics.EditAssociatedIdentifiers().RemoveIdentifier(key).Apply();
            }
            else
            {
                _module.UAirship.Analytics.EditAssociatedIdentifiers().AddIdentifier(key, identifier).Apply();
            }
            return Task.CompletedTask;
        }
    }
}