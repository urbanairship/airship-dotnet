using Microsoft.Maui.Controls.Hosting;
#if ANDROID || IOS
using AirshipDotNet.Embedded.Controls;
#endif

namespace AirshipDotNet.Embedded
{
    /// <summary>
    /// Extension methods for configuring Airship embedded views in MAUI applications.
    /// </summary>
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures the MAUI app to use Airship embedded view controls.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        /// <returns>The MAUI app builder for chaining.</returns>
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
