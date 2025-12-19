/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet.Analytics;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Analytics module.
    /// </summary>
    internal class AirshipAnalytics : IAirshipAnalytics
    {
        private readonly AirshipModule _module;

        internal AirshipAnalytics(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Tracks a custom event.
        /// </summary>
        /// <param name="customEvent">The custom event to track.</param>
        public Task TrackEvent(CustomEvent customEvent)
        {
            if (customEvent == null || string.IsNullOrEmpty(customEvent.EventName))
            {
                return Task.CompletedTask;
            }

            NSRunLoop.Main.BeginInvokeOnMainThread(() =>
            {
                var eventName = customEvent.EventName;
                var eventValue = customEvent.EventValue;
                var transactionId = customEvent.TransactionId;
                var interactionType = customEvent.InteractionType;
                var interactionId = customEvent.InteractionId;

                // SDK 19 uses double directly in the constructor
                UACustomEvent uaEvent;
                if (eventValue.HasValue)
                {
                    uaEvent = new UACustomEvent(eventName, eventValue.Value);
                }
                else
                {
                    // Single parameter constructor for events without value
                    uaEvent = new UACustomEvent(eventName);
                }

                if (!string.IsNullOrEmpty(transactionId))
                {
                    uaEvent.TransactionID = transactionId;
                }

                if (!string.IsNullOrEmpty(interactionId))
                {
                    uaEvent.InteractionID = interactionId;
                }

                if (!string.IsNullOrEmpty(interactionType))
                {
                    uaEvent.InteractionType = interactionType;
                }

                if (customEvent.PropertyList != null)
                {
                    foreach (dynamic property in customEvent.PropertyList)
                    {
                        if (string.IsNullOrEmpty(property.Name))
                        {
                            continue;
                        }

                        string key = property.Name;

                        if (property is CustomEvent.Property<string> stringProperty)
                        {
                            uaEvent.SetPropertyWithString(stringProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<bool> boolProperty)
                        {
                            uaEvent.SetPropertyWithBool(boolProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<int> intProperty)
                        {
                            uaEvent.SetPropertyWithDouble(intProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<long> longProperty)
                        {
                            uaEvent.SetPropertyWithDouble(longProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<float> floatProperty)
                        {
                            uaEvent.SetPropertyWithDouble(floatProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<double> doubleProperty)
                        {
                            uaEvent.SetPropertyWithDouble(doubleProperty.Value, key);
                        }
                        else if (property is CustomEvent.Property<string[]> stringArrayProperty)
                        {
                            // Arrays need to be set as JSON values
                            NSArray array = NSArray.FromObjects(stringArrayProperty.Value);
                            NSError? error;
                            uaEvent.SetPropertyWithValue(array, key, out error);
                        }
                    }
                }

                UAirship.Analytics.RecordCustomEvent(uaEvent);
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks a screen view.
        /// </summary>
        /// <param name="screen">The screen name.</param>
        public Task TrackScreen(string screen) 
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() => UAirship.Analytics.TrackScreen(screen));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Associates an identifier.
        /// </summary>
        /// <param name="key">The identifier key.</param>
        /// <param name="identifier">The identifier value.</param>
        public Task AssociateIdentifier(string key, string identifier)
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() =>
            {
                UAAssociatedIdentifiers identifiers = UAirship.Analytics.CurrentAssociatedDeviceIdentifiers;
                identifiers.SetWithIdentifier(identifier, key);
                UAirship.Analytics.AssociateDeviceIdentifier(identifiers);
            });
            return Task.CompletedTask;
        }
    }
}