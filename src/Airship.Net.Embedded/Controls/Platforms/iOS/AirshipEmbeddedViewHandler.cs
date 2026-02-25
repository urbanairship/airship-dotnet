using Microsoft.Maui.Handlers;
using UIKit;
using Foundation;
using Airship;

namespace AirshipDotNet.Embedded.Controls
{
    public partial class AirshipEmbeddedViewHandler : ViewHandler<AirshipEmbeddedView, UIView>
    {
        private UIView _containerView = null!;
        private UIViewController? _embeddedVC;
        private IDisposable? _sizeObservation;
        private bool _embedded;

        public AirshipEmbeddedViewHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        public AirshipEmbeddedViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
            : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper)
        {
        }

        protected override UIView CreatePlatformView()
        {
            _containerView = new UIView();
            return _containerView;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);
            EmbedIfNeeded();
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            _sizeObservation?.Dispose();
            _sizeObservation = null;
            _embeddedVC?.RemoveFromParentViewController();
            _embeddedVC?.View?.RemoveFromSuperview();
            _embeddedVC = null;
            _embedded = false;
            base.DisconnectHandler(platformView);
        }

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view)
        {
            if (handler is AirshipEmbeddedViewHandler h)
                h.EmbedIfNeeded();
        }

        // Walk up the responder chain from a view to find its nearest containing view controller.
        // This is more reliable than Platform.GetCurrentUIViewController(), which returns the
        // topmost presented VC (e.g. ShellFlyoutRenderer) rather than the page VC that hosts our view.
        private static UIViewController? FindContainerViewController(UIView view)
        {
            UIResponder? responder = view.NextResponder;
            while (responder != null)
            {
                if (responder is UIViewController vc)
                    return vc;
                responder = responder.NextResponder;
            }
            return null;
        }

        private void EmbedIfNeeded()
        {
            if (_embedded || string.IsNullOrEmpty(VirtualView?.EmbeddedId)) return;

            NSRunLoop.Main.BeginInvokeOnMainThread(() =>
            {
                if (_embedded || string.IsNullOrEmpty(VirtualView?.EmbeddedId)) return;

                var parentVC = FindContainerViewController(_containerView);
                if (parentVC == null) return;

                _embedded = true;
                var embeddedId = VirtualView!.EmbeddedId!;

                _embeddedVC = UAEmbeddedViewControllerFactory.MakeViewControllerWithEmbeddedID(embeddedId);
                parentVC.AddChildViewController(_embeddedVC);
                var embeddedView = _embeddedVC.View!;
                embeddedView.TranslatesAutoresizingMaskIntoConstraints = false;
                _containerView.AddSubview(embeddedView);

                // Pin top/leading/trailing only — not bottom.
                // UIHostingController with sizingOptions = .intrinsicContentSize reports the
                // SwiftUI content's natural height as its intrinsicContentSize. Pinning bottom
                // in addition would override that and cause content to be clipped.
                NSLayoutConstraint.ActivateConstraints([
                    embeddedView.TopAnchor.ConstraintEqualTo(_containerView.TopAnchor),
                    embeddedView.LeadingAnchor.ConstraintEqualTo(_containerView.LeadingAnchor),
                    embeddedView.TrailingAnchor.ConstraintEqualTo(_containerView.TrailingAnchor),
                ]);

                _embeddedVC.DidMoveToParentViewController(parentVC);

                // Observe preferredContentSize so height stays in sync whenever Airship content
                // appears, disappears, or changes size — not just at the 300 ms snapshot.
                _sizeObservation = _embeddedVC.AddObserver(
                    "preferredContentSize",
                    NSKeyValueObservingOptions.New,
                    _ => MainThread.BeginInvokeOnMainThread(FitToContent));

                // Initial fit after SwiftUI has had a chance to render.
                System.Threading.Tasks.Task.Delay(300).ContinueWith(_ =>
                    MainThread.BeginInvokeOnMainThread(FitToContent));
            });
        }

        private void FitToContent()
        {
            var h = _embeddedVC?.PreferredContentSize.Height ?? 0;
            if (VirtualView == null) return;

            // Collapse to zero when empty so the blank area doesn't take up space.
            VirtualView.HeightRequest = h > 0 ? h : 0;
        }
    }
}
