/* Copyright Airship and Contributors */

using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AirshipDotNet.Embedded.Controls
{
    /// <summary>Handler for <see cref="AirshipEmbeddedView"/>.</summary>
    public partial class AirshipEmbeddedViewHandler
    {
        public static IPropertyMapper<AirshipEmbeddedView, IViewHandler> PropertyMapper =
            new PropertyMapper<AirshipEmbeddedView, IViewHandler>(ViewHandler.ViewMapper)
            {
                [nameof(AirshipEmbeddedView.EmbeddedId)] = (handler, view) => MapEmbeddedId(handler, view)
            };

        public static CommandMapper<AirshipEmbeddedView, IViewHandler> CommandMapper =
            new CommandMapper<AirshipEmbeddedView, IViewHandler>(ViewHandler.ViewCommandMapper);

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view);
    }
}
