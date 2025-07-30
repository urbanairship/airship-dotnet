/* Copyright Airship and Contributors */

using System.Threading.Tasks;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Privacy Manager interface.
    /// </summary>
    public interface IAirshipPrivacyManager
    {
        /// <summary>
        /// Gets or sets the enabled features.
        /// </summary>
        Features EnabledFeatures { get; set; }

        /// <summary>
        /// Gets the currently enabled features.
        /// </summary>
        /// <returns>The enabled features.</returns>
        Task<Features> GetEnabledFeatures();

        /// <summary>
        /// Sets the enabled features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        Task SetEnabledFeatures(Features features);

        /// <summary>
        /// Enables specific features.
        /// </summary>
        /// <param name="features">The features to enable.</param>
        Task EnableFeatures(Features features);

        /// <summary>
        /// Disables specific features.
        /// </summary>
        /// <param name="features">The features to disable.</param>
        Task DisableFeatures(Features features);

        /// <summary>
        /// Checks if a specific feature is enabled.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True if the feature is enabled, false otherwise.</returns>
        Task<bool> IsFeatureEnabled(Features feature);

        /// <summary>
        /// Checks if any feature is enabled.
        /// </summary>
        /// <returns>True if any feature is enabled, false otherwise.</returns>
        Task<bool> IsAnyFeatureEnabled();
    }
}