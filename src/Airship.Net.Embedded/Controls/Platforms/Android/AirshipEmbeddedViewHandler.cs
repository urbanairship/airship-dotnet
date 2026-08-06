/* Copyright Airship and Contributors */

using System;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
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
            DetachNativeView();
            platformView.RemoveAllViews();
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

            DetachNativeView();
            PlatformView.RemoveAllViews();

            // Width fills the available space; height wraps the embedded content so the native
            // view measures to its natural height. FitToContent then pushes that height up to
            // the MAUI view, matching the iOS handler's intrinsic-content sizing.
            _nativeView = new NativeEmbeddedView(Context, id)
            {
                LayoutParameters = new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MatchParent,
                    FrameLayout.LayoutParams.WrapContent)
            };
            _nativeView.LayoutChange += OnNativeLayoutChange;
            PlatformView.AddView(_nativeView);
            _nativeViewId = id;
        }

        private void DetachNativeView()
        {
            if (_nativeView != null)
            {
                _nativeView.LayoutChange -= OnNativeLayoutChange;
                _nativeView = null;
            }
        }

        private void OnNativeLayoutChange(object? sender, Android.Views.View.LayoutChangeEventArgs e) => FitToContent();

        /// <summary>
        /// Measures the embedded content against an unconstrained height and pushes the result
        /// up to the MAUI view via HeightRequest, so the view grows to its content and
        /// collapses to zero when empty. Measuring unconstrained (rather than reading the
        /// laid-out height) avoids clipping the measurement to the current frame. Mirrors the
        /// iOS handler, which observes the hosting controller's intrinsic content size.
        /// </summary>
        private void FitToContent()
        {
            if (_nativeView == null || VirtualView == null || PlatformView == null)
                return;

            var widthPx = PlatformView.Width;
            if (widthPx <= 0)
                return; // Not laid out yet; a later layout pass will trigger another fit.

            _nativeView.Measure(
                Android.Views.View.MeasureSpec.MakeMeasureSpec(widthPx, MeasureSpecMode.Exactly),
                Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));

            var height = _nativeView.MeasuredHeight > 0
                ? Context.FromPixels(_nativeView.MeasuredHeight)
                : 0d;

            if (Math.Abs(VirtualView.HeightRequest - height) > 0.5)
                VirtualView.HeightRequest = height;
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
