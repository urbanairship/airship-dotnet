/* Copyright Airship and Contributors */

namespace AirshipDotNet
{
    /// <summary>Information about a pending embedded content placement.</summary>
    public class EmbeddedInfo
    {
        /// <summary>The embedded placement ID.</summary>
        public string EmbeddedId { get; }

        /// <summary>The unique instance ID of this pending content.</summary>
        public string InstanceId { get; }

        /// <summary>The content priority (lower displays first).</summary>
        public int Priority { get; }

        public EmbeddedInfo(string embeddedId, string instanceId, int priority)
        {
            EmbeddedId = embeddedId;
            InstanceId = instanceId;
            Priority = priority;
        }
    }
}
