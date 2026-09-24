/* Copyright Airship and Contributors */

using System;
using System.Linq;
using Airship;
using AirshipDotNet.AI;
using Foundation;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of <see cref="IAirshipAI"/>.
    /// </summary>
    public class AirshipAI : IAirshipAI
    {
        private readonly AirshipModule _module;

        // Held so the managed delegates survive as long as the native side holds the blocks.
        private readonly System.Collections.Generic.Dictionary<string, Func<string?, AirshipEvaluationContext>> _providers = new();
        private Func<AirshipEvaluationContext>? _defaultProvider;

        internal AirshipAI(AirshipModule module)
        {
            _module = module;
        }

        /// <inheritdoc />
        public bool SetContextProvider(string usage, Func<string?, AirshipEvaluationContext>? provider)
        {
            ArgumentNullException.ThrowIfNull(usage);

            lock (_providers)
            {
                if (provider == null)
                {
                    _providers.Remove(usage);
                }
                else
                {
                    _providers[usage] = provider;
                }
            }

            if (provider == null)
            {
                return UAAirshipAI.Shared.SetContextProvider(usage, null);
            }

            return UAAirshipAI.Shared.SetContextProvider(usage, subjectJson =>
            {
                Func<string?, AirshipEvaluationContext>? current;
                lock (_providers)
                {
                    _providers.TryGetValue(usage, out current);
                }

                var context = current?.Invoke(subjectJson) ?? AirshipEvaluationContext.Empty;
                return ToNativeItems(context);
            });
        }

        /// <inheritdoc />
        public void SetDefaultContextProvider(Func<AirshipEvaluationContext>? provider)
        {
            _defaultProvider = provider;

            if (provider == null)
            {
                UAAirshipAI.Shared.SetDefaultContextProvider(null);
                return;
            }

            UAAirshipAI.Shared.SetDefaultContextProvider(() =>
            {
                var context = _defaultProvider?.Invoke() ?? AirshipEvaluationContext.Empty;
                return ToNativeItems(context);
            });
        }

        private static UAEvaluationContextItem[] ToNativeItems(AirshipEvaluationContext context) =>
            context.Items
                .Select(item => new UAEvaluationContextItem(item.Content, item.Priority))
                .ToArray();
    }
}
