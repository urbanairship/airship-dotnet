/* Copyright Airship and Contributors */

using System.Threading.Tasks;
using AirshipDotNet.FeatureFlags;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Feature Flag Manager interface.
    /// </summary>
    public interface IAirshipFeatureFlagManager
    {
        /// <summary>
        /// Gets a feature flag by name.
        /// </summary>
        /// <param name="flagName">The flag name.</param>
        /// <returns>The feature flag or null if not found.</returns>
        Task<FeatureFlag?> GetFlag(string flagName);

        /// <summary>
        /// Tracks a feature flag interaction.
        /// </summary>
        /// <param name="flag">The feature flag to track.</param>
        Task TrackInteraction(FeatureFlag flag);
    }
}