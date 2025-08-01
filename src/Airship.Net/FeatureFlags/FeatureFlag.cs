/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;

namespace AirshipDotNet.FeatureFlags
{
    /// <summary>
    /// A Feature Flag model object.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FeatureFlag
    {
        /// <summary>
        /// Gets the flag name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets whether the flag exists.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// Gets whether the device is eligible for the flag.
        /// </summary>
        public bool IsEligible { get; }

        /// <summary>
        /// Gets the variables associated with the flag.
        /// </summary>
        public Dictionary<string, object?>? Variables { get; }

        /// <summary>
        /// Gets the reporting metadata.
        /// </summary>
        public Dictionary<string, object?>? ReportingMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureFlag"/> class.
        /// </summary>
        public FeatureFlag(string name, bool exists, bool isEligible, 
            Dictionary<string, object?>? variables = null,
            Dictionary<string, object?>? reportingMetadata = null)
        {
            Name = name;
            Exists = exists;
            IsEligible = isEligible;
            Variables = variables;
            ReportingMetadata = reportingMetadata;
        }
    }
}