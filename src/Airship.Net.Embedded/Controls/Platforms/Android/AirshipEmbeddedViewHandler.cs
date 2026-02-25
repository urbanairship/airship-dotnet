using Microsoft.Maui.Handlers;
using NativeEmbeddedView = Com.Urbanairship.Embedded.AirshipEmbeddedView;

namespace AirshipDotNet.Embedded.Controls
{
    public partial class AirshipEmbeddedViewHandler : ViewHandler<AirshipEmbeddedView, NativeEmbeddedView>
    {
        public AirshipEmbeddedViewHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        public AirshipEmbeddedViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
            : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper)
        {
        }

        protected override NativeEmbeddedView CreatePlatformView()
        {
            return new NativeEmbeddedView(Context, VirtualView!.EmbeddedId ?? "");
        }

        protected override void ConnectHandler(NativeEmbeddedView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(
                Android.Widget.RelativeLayout.LayoutParams.MatchParent,
                Android.Widget.RelativeLayout.LayoutParams.MatchParent);
        }

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view)
        {
            // EmbeddedId is passed at construction time; runtime changes are not supported.
        }
    }
}
