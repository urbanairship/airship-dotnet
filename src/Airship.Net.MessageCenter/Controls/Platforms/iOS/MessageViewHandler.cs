using System;
using Microsoft.Maui.Handlers;
using UIKit;
using Foundation;
using Airship;

namespace AirshipDotNet.MessageCenter.Controls
{
    public partial class MessageViewHandler : ViewHandler<MessageView, UIView>
    {
        private UAMessageCenterMessageViewController? _messageVC;
        private MessageContainerView? _containerView;

        public MessageViewHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        public MessageViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
            : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper)
        {
        }

        protected override UIView CreatePlatformView()
        {
            _containerView = new MessageContainerView();
            _containerView.BackgroundColor = UIColor.SystemBackground;
            _containerView.OnMovedToWindow = InstallMessageVC;
            return _containerView;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);

            if (VirtualView != null && !string.IsNullOrEmpty(VirtualView.MessageId))
                LoadMessage(VirtualView.MessageId);
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            UnloadMessageVC();
            if (_containerView != null)
            {
                _containerView.OnMovedToWindow = null;
                _containerView = null;
            }
            base.DisconnectHandler(platformView);
        }

        static partial void MapMessageId(IViewHandler handler, MessageView view)
        {
            if (!string.IsNullOrEmpty(view.MessageId) && handler is MessageViewHandler h)
                h.LoadMessage(view.MessageId);
        }

        private void LoadMessage(string messageId)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Swap out any existing message VC before installing a new one
                if (_messageVC != null)
                    UnloadMessageVC();

                VirtualView?.SendLoadStarted();

                _messageVC = new UAMessageCenterMessageViewController(messageId);

                // Install immediately if the container is already in the window hierarchy;
                // otherwise MovedToWindow will trigger installation.
                if (_containerView?.Window != null)
                    InstallMessageVC();
            });
        }

        private void InstallMessageVC()
        {
            if (_messageVC == null || _containerView == null)
                return;

            // Guard against double-installation
            if (_messageVC.ParentViewController != null)
                return;

            var parentVC = FindViewController(_containerView);
            if (parentVC == null)
                return;

            parentVC.AddChildViewController(_messageVC);

            var vcView = _messageVC.View!;
            vcView.TranslatesAutoresizingMaskIntoConstraints = false;
            _containerView.AddSubview(vcView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                vcView.TopAnchor.ConstraintEqualTo(_containerView.TopAnchor),
                vcView.LeadingAnchor.ConstraintEqualTo(_containerView.LeadingAnchor),
                vcView.TrailingAnchor.ConstraintEqualTo(_containerView.TrailingAnchor),
                vcView.BottomAnchor.ConstraintEqualTo(_containerView.BottomAnchor),
            });

            _messageVC.DidMoveToParentViewController(parentVC);
            VirtualView?.SendLoadFinished();
        }

        private void UnloadMessageVC()
        {
            if (_messageVC == null)
                return;

            _messageVC.WillMoveToParentViewController(null);
            _messageVC.View?.RemoveFromSuperview();
            _messageVC.RemoveFromParentViewController();
            _messageVC.Dispose();
            _messageVC = null;
        }

        private static UIViewController? FindViewController(UIView view)
        {
            var responder = view.NextResponder;
            while (responder != null)
            {
                if (responder is UIViewController vc)
                    return vc;
                responder = responder.NextResponder;
            }
            return null;
        }

        // UIView subclass that notifies when it enters the window hierarchy
        private sealed class MessageContainerView : UIView
        {
            internal Action? OnMovedToWindow;

            public override void MovedToWindow()
            {
                base.MovedToWindow();
                if (Window != null)
                    OnMovedToWindow?.Invoke();
            }
        }
    }
}
