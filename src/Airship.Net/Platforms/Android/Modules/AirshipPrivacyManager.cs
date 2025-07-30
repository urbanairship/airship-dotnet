/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrbanAirship;
using UrbanAirship.Push;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship PrivacyManager module.
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
            get
            {
                var privacyManager = _module.UAirship.PrivacyManager;
                Features features = Features.None;

                // Check each feature individually using IsEnabled
                if (privacyManager.IsEnabled(PrivacyManager.Feature.InAppAutomation))
                    features |= Features.InAppAutomation;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.MessageCenter))
                    features |= Features.MessageCenter;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.Push))
                    features |= Features.Push;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.Analytics))
                    features |= Features.Analytics;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.TagsAndAttributes))
                    features |= Features.TagsAndAttributes;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.Contacts))
                    features |= Features.Contacts;
                if (privacyManager.IsEnabled(PrivacyManager.Feature.FeatureFlags))
                    features |= Features.FeatureFlags;

                return features;
            }
            set => _module.UAirship.PrivacyManager.SetEnabledFeatures(UAFeaturesFromFeatures(value));
        }

        /// <summary>
        /// Gets the currently enabled features.
        /// </summary>
        /// <returns>The enabled features.</returns>
        public Task<Features> GetEnabledFeatures()
        {
            return Task.FromResult(EnabledFeatures);
        }

        /// <summary>
        /// Sets the enabled features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        public Task SetEnabledFeatures(Features features)
        {
            EnabledFeatures = features;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Enables specific features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        public Task EnableFeatures(Features features)
        {
            _module.UAirship.PrivacyManager.Enable(UAFeaturesFromFeatures(features));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disables specific features.
        /// </summary>
        /// <param name="features">The features to disable.</param>
        public Task DisableFeatures(Features features)
        {
            _module.UAirship.PrivacyManager.Disable(UAFeaturesFromFeatures(features));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Checks if a specific feature is enabled.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        public Task<bool> IsFeatureEnabled(Features feature)
        {
            return Task.FromResult(EnabledFeatures.HasFlag(feature));
        }

        /// <summary>
        /// Checks if any feature is enabled.
        /// </summary>
        /// <returns>True if any feature is enabled, false otherwise.</returns>
        public Task<bool> IsAnyFeatureEnabled()
        {
            return Task.FromResult(EnabledFeatures != Features.None);
        }

        private static PrivacyManager.Feature[] UAFeaturesFromFeatures(Features features)
        {
            var uaFeatures = new List<PrivacyManager.Feature>();

            if (features.HasFlag(Features.InAppAutomation))
            {
                uaFeatures.Add(PrivacyManager.Feature.InAppAutomation);
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                uaFeatures.Add(PrivacyManager.Feature.MessageCenter);
            }
            if (features.HasFlag(Features.Push))
            {
                uaFeatures.Add(PrivacyManager.Feature.Push);
            }
            if (features.HasFlag(Features.Analytics))
            {
                uaFeatures.Add(PrivacyManager.Feature.Analytics);
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                uaFeatures.Add(PrivacyManager.Feature.TagsAndAttributes);
            }
            if (features.HasFlag(Features.Contacts))
            {
                uaFeatures.Add(PrivacyManager.Feature.Contacts);
            }
            if (features.HasFlag(Features.FeatureFlags))
            {
                uaFeatures.Add(PrivacyManager.Feature.FeatureFlags);
            }

            return uaFeatures.ToArray();
        }

        private static Features FeaturesFromUAFeatures(PrivacyManager.Feature[] uaFeatures)
        {
            Features features = Features.None;

            if (uaFeatures.Contains(PrivacyManager.Feature.InAppAutomation))
            {
                features |= Features.InAppAutomation;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.MessageCenter))
            {
                features |= Features.MessageCenter;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.Push))
            {
                features |= Features.Push;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.Analytics))
            {
                features |= Features.Analytics;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.TagsAndAttributes))
            {
                features |= Features.TagsAndAttributes;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.Contacts))
            {
                features |= Features.Contacts;
            }
            if (uaFeatures.Contains(PrivacyManager.Feature.FeatureFlags))
            {
                features |= Features.FeatureFlags;
            }

            return features;
        }
    }
}