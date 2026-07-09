/* Copyright Airship and Contributors */

using System;
using Android.Widget;
using Microsoft.Maui.Handlers;
using NativeEmbeddedView = Com.Urbanairship.Embedded.AirshipEmbeddedView;

namespace AirshipDotNet.Embedded.Controls
{
    public partial class AirshipEmbeddedViewHandler : ViewHandler<AirshipEmbeddedView, FrameLayout>
    {
        private NativeEmbeddedView? _nativeView;
        private string? _nativeViewId;
        private EventHandler<AirshipDotNet.EmbeddedInfoUpdatedEventArgs>? _availabilityHandler;

        public AirshipEmbeddedViewHandler() : base(PropertyMapper, CommandMapper) { }
        public AirshipEmbeddedViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
            : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper) { }

        protected override FrameLayout CreatePlatformView() => new FrameLayout(Context);

        protected override void ConnectHandler(FrameLayout platformView)
        {
            base.ConnectHandler(platformView);
            _availabilityHandler = (_, _) => UpdateCollapse();
            AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated += _availabilityHandler;
            UpdateNativeView();
            UpdateCollapse();
        }

        protected override void DisconnectHandler(FrameLayout platformView)
        {
            if (_availabilityHandler != null)
            {
                AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated -= _availabilityHandler;
                _availabilityHandler = null;
            }
            platformView.RemoveAllViews();
            _nativeView = null;
            _nativeViewId = null;
            base.DisconnectHandler(platformView);
        }

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view)
        {
            if (handler is AirshipEmbeddedViewHandler h)
            {
                h.UpdateNativeView();
                h.UpdateCollapse();
            }
        }

        /// <summary>
        /// Creates (or recreates) the native embedded view for the current EmbeddedId. The
        /// native view is bound to a single ID at construction, so ID changes require a
        /// new instance.
        /// </summary>
        private void UpdateNativeView()
        {
            var id = VirtualView?.EmbeddedId;
            if (PlatformView == null || string.IsNullOrEmpty(id) || id == _nativeViewId)
                return;

            PlatformView.RemoveAllViews();
            _nativeView = new NativeEmbeddedView(Context, id)
            {
                LayoutParameters = new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MatchParent,
                    FrameLayout.LayoutParams.MatchParent)
            };
            PlatformView.AddView(_nativeView);
            _nativeViewId = id;
        }

        private void UpdateCollapse()
        {
            var id = VirtualView?.EmbeddedId;
            if (VirtualView == null || string.IsNullOrEmpty(id))
                return;

            VirtualView.IsVisible = AirshipDotNet.Airship.InApp.IsEmbeddedAvailable(id);
        }
    }
}
