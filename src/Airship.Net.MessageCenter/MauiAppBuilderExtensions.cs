using Microsoft.Maui.Controls.Hosting;
#if ANDROID || IOS
using AirshipDotNet.MessageCenter.Controls;
#endif

namespace AirshipDotNet.MessageCenter
{
    /// <summary>
    /// Extension methods for configuring Airship in MAUI applications.
    /// </summary>
    public static class MauiAppBuilderExtensions
    {
        /// <summary>
        /// Configures the MAUI app to use Airship Message Center controls.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        /// <returns>The MAUI app builder for chaining.</returns>
        public static MauiAppBuilder UseAirshipMessageCenter(this MauiAppBuilder builder)
        {
#if ANDROID || IOS
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(MessageView), typeof(MessageViewHandler));
            });
#endif
            return builder;
        }
    }
}