/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using AirshipDotNet.FeatureFlags;

namespace AirshipDotNet
{
    /// <summary>
    /// iOS implementation of Airship FeatureFlagManager module.
    /// NOTE: Feature Flags are not currently exposed in the AirshipObjectiveC xcframework.
    /// This is a placeholder implementation that returns null/no-op results.
    /// </summary>
    internal class AirshipFeatureFlagManager : IAirshipFeatureFlagManager
    {
        private readonly AirshipModule _module;

        internal AirshipFeatureFlagManager(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets a feature flag by name.
        /// </summary>
        /// <param name="flagName">The flag name.</param>
        /// <returns>Always returns null as feature flags are not available on iOS.</returns>
        public Task<FeatureFlag?> GetFlag(string flagName)
        {
            // TODO: Feature Flags are not exposed in the AirshipObjectiveC xcframework.
            // This will need to be implemented when the iOS SDK exposes this functionality.
            return Task.FromResult<FeatureFlag?>(null);
        }

        /// <summary>
        /// Tracks a feature flag interaction.
        /// </summary>
        /// <param name="flag">The feature flag to track.</param>
        /// <returns>A completed task (no-op).</returns>
        public Task TrackInteraction(FeatureFlag flag)
        {
            // TODO: Feature Flags are not exposed in the AirshipObjectiveC xcframework.
            // This will need to be implemented when the iOS SDK exposes this functionality.
            return Task.CompletedTask;
        }
    }
}