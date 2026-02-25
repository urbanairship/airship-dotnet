using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AirshipDotNet.Embedded.Controls
{
    /// <summary>
    /// Handler for AirshipEmbeddedView control.
    /// </summary>
    public partial class AirshipEmbeddedViewHandler
    {
        /// <summary>
        /// Property mapper for AirshipEmbeddedView.
        /// </summary>
        public static IPropertyMapper<AirshipEmbeddedView, IViewHandler> PropertyMapper = new PropertyMapper<AirshipEmbeddedView, IViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(AirshipEmbeddedView.EmbeddedId)] = (handler, view) => MapEmbeddedId(handler, view)
        };

        /// <summary>
        /// Command mapper for AirshipEmbeddedView.
        /// </summary>
        public static CommandMapper<AirshipEmbeddedView, IViewHandler> CommandMapper = new CommandMapper<AirshipEmbeddedView, IViewHandler>(ViewHandler.ViewCommandMapper);

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view);
    }
}
