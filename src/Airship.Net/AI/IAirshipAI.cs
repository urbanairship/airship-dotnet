/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet.AI
{
    /// <summary>
    /// Stable usage keys identifying an AI use case. One key per SDK feature that can be
    /// evaluated. Apps do not declare usages; pass one of these constants.
    /// </summary>
    public static class AirshipAIUsage
    {
        /// <summary>
        /// Deciding whether to suppress an in-app message.
        /// The subject JSON carries <c>name</c>, <c>priority</c>, <c>hints</c>, and optionally <c>extras</c>.
        /// </summary>
        public const string InAppMessageSuppression = "in_app_message_suppression";

        /// <summary>
        /// Choosing which pending embedded content to show.
        /// </summary>
        public const string EmbeddedSelection = "embedded_selection";
    }

    /// <summary>
    /// Entry point for supplying app context to Airship's on-device AI evaluations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Gated by <see cref="Features.OnDeviceAI"/>. When that feature is disabled no model
    /// resolves and every evaluation is skipped, so providers are never invoked.
    /// </para>
    /// <para>
    /// Providers are called immediately before each evaluation, on the path to displaying the
    /// feature, so keep the work light and synchronous. Supplying a custom model is not
    /// exposed through this plugin — it requires generation schemas and structured output that
    /// do not cross the native boundary usefully.
    /// </para>
    /// </remarks>
    public interface IAirshipAI
    {
        /// <summary>
        /// Registers the context provider for a usage, replacing any previous one.
        /// </summary>
        /// <param name="usage">A key from <see cref="AirshipAIUsage"/>.</param>
        /// <param name="provider">
        /// Receives the feature-specific subject as a JSON string (null when the usage carries
        /// no subject payload) and returns the context to contribute. Pass <c>null</c> to clear.
        /// </param>
        /// <returns>
        /// <c>true</c> if the usage is recognized by the underlying SDK and the provider was
        /// applied; <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// A provider registered here <b>replaces</b> <see cref="SetDefaultContextProvider"/>
        /// for this usage rather than adding to it — the two are never combined. Merge the
        /// general context in yourself if you want both.
        /// </remarks>
        bool SetContextProvider(string usage, Func<string?, AirshipEvaluationContext>? provider);

        /// <summary>
        /// Registers the fallback context provider, used for usages with no provider of their own.
        /// </summary>
        /// <param name="provider">The provider, or <c>null</c> to clear it.</param>
        void SetDefaultContextProvider(Func<AirshipEvaluationContext>? provider);
    }
}
