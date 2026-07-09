/* Copyright Airship and Contributors */

using System;
using Microsoft.Maui.Handlers;
using UIKit;
using Foundation;
using Airship;

namespace AirshipDotNet.Embedded.Controls
{
    public partial class AirshipEmbeddedViewHandler : ViewHandler<AirshipEmbeddedView, UIView>
    {
        private EmbeddedContainerView? _containerView;
        private UIViewController? _embeddedVC;
        private bool _installed;
        private string? _installedId;
        private EventHandler<AirshipDotNet.EmbeddedInfoUpdatedEventArgs>? _availabilityHandler;

        public AirshipEmbeddedViewHandler() : base(PropertyMapper, CommandMapper) { }
        public AirshipEmbeddedViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
            : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper) { }

        protected override UIView CreatePlatformView()
        {
            _containerView = new EmbeddedContainerView { OnMovedToWindow = InstallIfNeeded };
            return _containerView;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);
            _availabilityHandler = (_, _) => UpdateCollapse();
            AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated += _availabilityHandler;
            UpdateCollapse();
            InstallIfNeeded();
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            if (_availabilityHandler != null)
            {
                AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated -= _availabilityHandler;
                _availabilityHandler = null;
            }
            Uninstall();
            if (_containerView != null) { _containerView.OnMovedToWindow = null; _containerView = null; }
            base.DisconnectHandler(platformView);
        }

        static partial void MapEmbeddedId(IViewHandler handler, AirshipEmbeddedView view)
        {
            if (handler is AirshipEmbeddedViewHandler h)
            {
                // The embedded view controller is bound to a single ID; swap it on change.
                if (h._installed && h._installedId != view.EmbeddedId) h.Uninstall();
                h.UpdateCollapse();
                h.InstallIfNeeded();
            }
        }

        private void Uninstall()
        {
            _embeddedVC?.WillMoveToParentViewController(null);
            _embeddedVC?.View?.RemoveFromSuperview();
            _embeddedVC?.RemoveFromParentViewController();
            _embeddedVC = null;
            _installed = false;
            _installedId = null;
        }

        private void InstallIfNeeded()
        {
            if (_installed || _containerView == null) return;
            var embeddedId = VirtualView?.EmbeddedId;
            if (string.IsNullOrEmpty(embeddedId) || _containerView.Window == null) return;

            var parentVC = FindViewController(_containerView);
            if (parentVC == null) return;

            _installed = true;
            _installedId = embeddedId;
            _embeddedVC = UAEmbeddedViewControllerFactory.MakeViewControllerWithEmbeddedID(embeddedId);
            parentVC.AddChildViewController(_embeddedVC);
            var v = _embeddedVC.View!;
            v.TranslatesAutoresizingMaskIntoConstraints = false;
            _containerView.AddSubview(v);
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                v.TopAnchor.ConstraintEqualTo(_containerView.TopAnchor),
                v.LeadingAnchor.ConstraintEqualTo(_containerView.LeadingAnchor),
                v.TrailingAnchor.ConstraintEqualTo(_containerView.TrailingAnchor),
                v.BottomAnchor.ConstraintEqualTo(_containerView.BottomAnchor),
            });
            _embeddedVC.DidMoveToParentViewController(parentVC);
        }

        private void UpdateCollapse()
        {
            var id = VirtualView?.EmbeddedId;
            if (VirtualView == null || string.IsNullOrEmpty(id)) return;
            // Let content drive height when available; collapse when not.
            VirtualView.IsVisible = AirshipDotNet.Airship.InApp.IsEmbeddedAvailable(id);
        }

        private static UIViewController? FindViewController(UIView view)
        {
            var r = view.NextResponder;
            while (r != null) { if (r is UIViewController vc) return vc; r = r.NextResponder; }
            return null;
        }

        private sealed class EmbeddedContainerView : UIView
        {
            internal Action? OnMovedToWindow;
            public override void MovedToWindow()
            {
                base.MovedToWindow();
                if (Window != null) OnMovedToWindow?.Invoke();
            }
        }
    }
}
