/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Privacy Manager module.
    /// </summary>
    internal class AirshipPrivacyManager : IAirshipPrivacyManager
    {
        private readonly AirshipModule _module;

        internal AirshipPrivacyManager(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets or sets the enabled features.
        /// </summary>
        public Features EnabledFeatures
        {
            get => FeaturesFromUAFeature(UAirship.PrivacyManager.EnabledFeatures);
            set => UAirship.PrivacyManager.EnabledFeatures = UAFeatureFromFeatures(value);
        }

        /// <summary>
        /// Gets the currently enabled features.
        /// </summary>
        /// <returns>The enabled features.</returns>
        public Task<Features> GetEnabledFeaturesAsync()
        {
            return Task.Run(() =>
            {
                var uaFeatures = UAirship.PrivacyManager.EnabledFeatures;
                return FeaturesFromUAFeature(uaFeatures);
            });
        }

        /// <summary>
        /// Sets the enabled features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        public Task SetEnabledFeaturesAsync(Features features)
        {
            return Task.Run(() =>
            {
                UAirship.PrivacyManager.EnabledFeatures = UAFeatureFromFeatures(features);
            });
        }

        /// <summary>
        /// Enables specific features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        public Task EnableFeaturesAsync(Features features)
        {
            return Task.Run(() =>
            {
                UAirship.PrivacyManager.EnableFeatures(UAFeatureFromFeatures(features));
            });
        }

        /// <summary>
        /// Disables specific features.
        /// </summary>
        /// <param name="features">The features to disable.</param>
        public Task DisableFeaturesAsync(Features features)
        {
            return Task.Run(() =>
            {
                UAirship.PrivacyManager.DisableFeatures(UAFeatureFromFeatures(features));
            });
        }

        /// <summary>
        /// Checks if a specific feature is enabled.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        public async Task<bool> IsFeatureEnabledAsync(Features feature)
        {
            var enabledFeatures = await GetEnabledFeaturesAsync();
            return enabledFeatures.HasFlag(feature);
        }

        /// <summary>
        /// Checks if any feature is enabled.
        /// </summary>
        /// <returns>True if any feature is enabled, false otherwise.</returns>
        public async Task<bool> IsAnyFeatureEnabledAsync()
        {
            var enabledFeatures = await GetEnabledFeaturesAsync();
            return enabledFeatures != Features.None;
        }

        private static UAFeature UAFeatureFromFeatures(Features features)
        {
            var featureList = new List<UAFeature>();

            if (features.HasFlag(Features.InAppAutomation))
            {
                featureList.Add(UAFeature.InAppAutomation);
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                featureList.Add(UAFeature.MessageCenter);
            }
            if (features.HasFlag(Features.Push))
            {
                featureList.Add(UAFeature.Push);
            }
            if (features.HasFlag(Features.Analytics))
            {
                featureList.Add(UAFeature.Analytics);
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                featureList.Add(UAFeature.TagsAndAttributes);
            }
            if (features.HasFlag(Features.Contacts))
            {
                featureList.Add(UAFeature.Contacts);
            }

            if (featureList.Count == 0)
            {
                return UAFeature.None;
            }

            return new UAFeature(featureList.ToArray());
        }

        private static Features FeaturesFromUAFeature(UAFeature uaFeature)
        {
            Features features = Features.None;

            if (uaFeature.Contains(UAFeature.InAppAutomation))
            {
                features |= Features.InAppAutomation;
            }
            if (uaFeature.Contains(UAFeature.MessageCenter))
            {
                features |= Features.MessageCenter;
            }
            if (uaFeature.Contains(UAFeature.Push))
            {
                features |= Features.Push;
            }
            if (uaFeature.Contains(UAFeature.Analytics))
            {
                features |= Features.Analytics;
            }
            if (uaFeature.Contains(UAFeature.TagsAndAttributes))
            {
                features |= Features.TagsAndAttributes;
            }
            if (uaFeature.Contains(UAFeature.Contacts))
            {
                features |= Features.Contacts;
            }

            return features;
        }
    }
}