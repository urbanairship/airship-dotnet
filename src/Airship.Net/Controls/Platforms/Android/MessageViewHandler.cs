using System;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UrbanAirship.MessageCenter;
using Android.Webkit;
using Com.Urbanairship.Messagecenter.UI.Widget;
using AView = Android.Views.View;
using AContext = Android.Content.Context;
using JavaObject = Java.Lang.Object;

namespace AirshipDotNet.Controls
{
    public partial class MessageViewHandler : ViewHandler<MessageView, FrameLayout>
    {
        private FrameLayout _container;
        private MessageWebView _webView;

        protected override FrameLayout CreatePlatformView()
        {
            _container = new FrameLayout(Context);
            
            // Create specialized MessageWebView
            _webView = new MessageWebView(Context);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.DomStorageEnabled = true;
            _webView.Settings.LoadWithOverviewMode = true;
            _webView.Settings.UseWideViewPort = true;
            
            // Set custom WebViewClient that extends MessageWebViewClient to monitor loading
            _webView.SetWebViewClient(new CustomMessageWebViewClient(this));
            
            // Add WebView to container
            _container.AddView(_webView, new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent));
            
            return _container;
        }

        protected override void ConnectHandler(FrameLayout platformView)
        {
            base.ConnectHandler(platformView);

            if (VirtualView != null && !string.IsNullOrEmpty(VirtualView.MessageId))
            {
                LoadMessage(VirtualView.MessageId);
            }
        }

        protected override void DisconnectHandler(FrameLayout platformView)
        {
            _webView?.Destroy();
            _webView = null;
            base.DisconnectHandler(platformView);
        }

        static partial void MapMessageId(MessageViewHandler handler, MessageView view)
        {
            if (!string.IsNullOrEmpty(view.MessageId))
            {
                handler.LoadMessage(view.MessageId);
            }
        }

        private void LoadMessage(string messageId)
        {
            try
            {
                VirtualView?.SendLoadStarted();

                // Get the message
                MessageCenterClass.Shared().Inbox.GetMessage(messageId, message =>
                {
                    if (message == null)
                    {
                        VirtualView?.SendLoadFailed("Message not found");
                        return;
                    }

                    // Mark as read
                    MessageCenterClass.Shared().Inbox.MarkMessagesRead(new[] { messageId });

                    // Use the specialized LoadMessage method which handles authentication
                    _webView.LoadMessage(message);
                });
            }
            catch (Exception ex)
            {
                VirtualView?.SendLoadFailed(ex.Message);
            }
        }

        private class CustomMessageWebViewClient : Com.Urbanairship.Messagecenter.UI.Widget.MessageWebViewClient
        {
            private readonly MessageViewHandler _handler;

            public CustomMessageWebViewClient(MessageViewHandler handler)
            {
                _handler = handler;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
                _handler.VirtualView?.SendLoadFinished();
            }

            public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
            {
                base.OnReceivedError(view, request, error);
                _handler.VirtualView?.SendLoadFailed(error.Description?.ToString() ?? "Unknown error");
            }

            [Obsolete]
            public override void OnReceivedError(Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
            {
                base.OnReceivedError(view, errorCode, description, failingUrl);
                _handler.VirtualView?.SendLoadFailed(description);
            }
        }
    }
}