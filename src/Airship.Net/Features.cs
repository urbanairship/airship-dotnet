/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet
{
    /// <summary>
    /// Feature flags for Airship privacy management.
    /// </summary>
    [Flags]
    public enum Features
    {
        /// <summary>
        /// No features enabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// In-app automation feature.
        /// </summary>
        InAppAutomation = 1 << 0,

        /// <summary>
        /// Message center feature.
        /// </summary>
        MessageCenter = 1 << 1,

        /// <summary>
        /// Push notifications feature.
        /// </summary>
        Push = 1 << 2,

        /// <summary>
        /// Analytics feature.
        /// </summary>
        Analytics = 1 << 4,

        /// <summary>
        /// Tags and attributes feature.
        /// </summary>
        TagsAndAttributes = 1 << 5,

        /// <summary>
        /// Contacts feature.
        /// </summary>
        Contacts = 1 << 6,

        /// <summary>
        /// Feature flags feature.
        /// </summary>
        FeatureFlags = 1 << 8,

        /// <summary>
        /// All features enabled.
        /// </summary>
        All = InAppAutomation | MessageCenter | Push | Analytics | TagsAndAttributes | Contacts | FeatureFlags
    }
}