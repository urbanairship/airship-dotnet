using System;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UrbanAirship.MessageCenter;
using Android.Webkit;
using AView = Android.Views.View;
using AContext = Android.Content.Context;
using JavaObject = Java.Lang.Object;

namespace AirshipDotNet.Controls
{
    public partial class MessageViewHandler : ViewHandler<MessageView, FrameLayout>
    {
        private FrameLayout _container;
        private Android.Webkit.WebView _webView;

        protected override FrameLayout CreatePlatformView()
        {
            _container = new FrameLayout(Context);
            
            // Create WebView
            _webView = new Android.Webkit.WebView(Context);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.DomStorageEnabled = true;
            _webView.Settings.LoadWithOverviewMode = true;
            _webView.Settings.UseWideViewPort = true;
            
            // Set WebViewClient to monitor loading
            _webView.SetWebViewClient(new MessageWebViewClient(this));
            
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

                    // Load message URL
                    if (!string.IsNullOrEmpty(message.BodyUrl))
                    {
                        _webView.LoadUrl(message.BodyUrl);
                    }
                    else
                    {
                        VirtualView?.SendLoadFailed("No message URL available");
                    }
                });
            }
            catch (Exception ex)
            {
                VirtualView?.SendLoadFailed(ex.Message);
            }
        }

        private class MessageWebViewClient : WebViewClient
        {
            private readonly MessageViewHandler _handler;

            public MessageWebViewClient(MessageViewHandler handler)
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