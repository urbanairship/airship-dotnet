/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Linq;
using AirshipDotNet.AI;
using Com.Urbanairship.AI;
using Kotlin.Coroutines;
using NativeAirship = UrbanAirship.Airship;
using NativeContext = Com.Urbanairship.AI.EvaluationContext;

namespace AirshipDotNet.Platforms.Android.Modules
{
    /// <summary>
    /// Android implementation of <see cref="IAirshipAI"/>.
    /// </summary>
    /// <remarks>
    /// The native provider interfaces declare Kotlin <c>suspend</c> functions, which the binding
    /// generator surfaces as a method taking a trailing <see cref="IContinuation"/> and returning
    /// <see cref="Java.Lang.Object"/>. Returning the value directly (rather than the
    /// COROUTINE_SUSPENDED marker) is how a suspend function that completes without suspending
    /// behaves, which is why the managed callbacks are synchronous.
    /// </remarks>
    public class AirshipAI : IAirshipAI
    {
        private readonly AirshipModule _module;

        // Held so the wrappers are not collected while the native side holds them.
        private readonly Dictionary<string, ContextProviderWrapper> _providers = new();
        private DefaultContextProviderWrapper? _defaultProvider;

        internal AirshipAI(AirshipModule module)
        {
            _module = module;
        }

        /// <inheritdoc />
        public bool SetContextProvider(string usage, Func<string?, AirshipEvaluationContext>? provider)
        {
            ArgumentNullException.ThrowIfNull(usage);

            // Usage's constructor is @RestrictTo, but the raw values are the stable wire keys,
            // so constructing one directly avoids depending on the per-feature extension
            // properties, which bind awkwardly as companion-object extensions.
            var nativeUsage = new Usage(usage);

            if (provider == null)
            {
                lock (_providers)
                {
                    _providers.Remove(usage);
                }

                NativeAirship.Ai.SetContextProvider(nativeUsage, null);
                return true;
            }

            var wrapper = new ContextProviderWrapper(provider);
            lock (_providers)
            {
                _providers[usage] = wrapper;
            }

            NativeAirship.Ai.SetContextProvider(nativeUsage, wrapper);
            return true;
        }

        /// <inheritdoc />
        public void SetDefaultContextProvider(Func<AirshipEvaluationContext>? provider)
        {
            if (provider == null)
            {
                _defaultProvider = null;
                NativeAirship.Ai.SetDefaultContextProvider(null);
                return;
            }

            _defaultProvider = new DefaultContextProviderWrapper(provider);
            NativeAirship.Ai.SetDefaultContextProvider(_defaultProvider);
        }

        private static NativeContext ToNativeContext(AirshipEvaluationContext context)
        {
            var items = context.Items
                .Select(item => new NativeContext.Item(item.Content, item.Priority))
                .ToList();

            return new NativeContext(items);
        }

        /// <summary>
        /// Serializes the native subject so managed code sees the same JSON shape as on iOS.
        /// </summary>
        private static string? SubjectToJson(Java.Lang.Object? subject) =>
            subject switch
            {
                null => null,
                // Subjects are Kotlin data classes; their toString is not JSON, so prefer the
                // JSON representation when the subject exposes one.
                global::UrbanAirship.Json.IJsonSerializable serializable => serializable.ToJsonValue()?.ToString(),
                _ => subject.ToString()
            };

        private class ContextProviderWrapper : Java.Lang.Object, IEvaluationContextProvider
        {
            private readonly Func<string?, AirshipEvaluationContext> _provider;

            internal ContextProviderWrapper(Func<string?, AirshipEvaluationContext> provider)
            {
                _provider = provider;
            }

            public Java.Lang.Object? ProvideContext(Java.Lang.Object? subject, IContinuation p1)
            {
                var context = _provider(SubjectToJson(subject)) ?? AirshipEvaluationContext.Empty;
                return ToNativeContext(context);
            }
        }

        private class DefaultContextProviderWrapper : Java.Lang.Object, IDefaultEvaluationContextProvider
        {
            private readonly Func<AirshipEvaluationContext> _provider;

            internal DefaultContextProviderWrapper(Func<AirshipEvaluationContext> provider)
            {
                _provider = provider;
            }

            public Java.Lang.Object? ProvideContext(IContinuation p0)
            {
                var context = _provider() ?? AirshipEvaluationContext.Empty;
                return ToNativeContext(context);
            }
        }
    }
}
