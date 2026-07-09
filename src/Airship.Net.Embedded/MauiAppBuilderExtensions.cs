/* Copyright Airship and Contributors */

using Microsoft.Maui.Controls.Hosting;
#if ANDROID || IOS
using AirshipDotNet.Embedded.Controls;
#endif

namespace AirshipDotNet.Embedded
{
    /// <summary>Extension methods for configuring Airship embedded views in MAUI apps.</summary>
    public static class MauiAppBuilderExtensions
    {
        /// <summary>Registers the Airship embedded view handler.</summary>
        public static MauiAppBuilder UseAirshipEmbedded(this MauiAppBuilder builder)
        {
#if ANDROID || IOS
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(AirshipEmbeddedView), typeof(AirshipEmbeddedViewHandler));
            });
#endif
            return builder;
        }
    }
}
