using System;
using Microsoft.Maui.Handlers;
using Com.Urbanairship.Messagecenter.UI.Widget;
using UrbanAirship.MessageCenter;
using Android.Webkit;

namespace AirshipDotNet.Controls
{
    public partial class MessageViewHandler : ViewHandler<MessageView, MessageWebView>
    {
        private MessageWebView _webView;

        protected override MessageWebView CreatePlatformView()
        {
            _webView = new MessageWebView(Context);
            
            // Configure WebView settings
            var settings = _webView.Settings;
            settings.JavaScriptEnabled = true;
            settings.DomStorageEnabled = true;
            
            _webView.SetWebViewClient(new CustomMessageWebViewClient(this));
            
            return _webView;
        }

        protected override void ConnectHandler(MessageWebView platformView)
        {
            base.ConnectHandler(platformView);

            // Ensure WebView has proper layout parameters
            platformView.LayoutParameters = new Android.Widget.FrameLayout.LayoutParams(
                Android.Widget.FrameLayout.LayoutParams.MatchParent,
                Android.Widget.FrameLayout.LayoutParams.MatchParent);

            if (VirtualView != null && !string.IsNullOrEmpty(VirtualView.MessageId))
            {
                LoadMessage(VirtualView.MessageId);
            }
        }

        protected override void DisconnectHandler(MessageWebView platformView)
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
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            VirtualView?.SendLoadFailed("Message not found");
                        });
                        return;
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Mark as read
                        MessageCenterClass.Shared().Inbox.MarkMessagesRead(new[] { messageId });

                        // Use the specialized LoadMessage method which handles authentication
                        _webView.LoadMessage(message);
                    });
                });
            }
            catch (Exception ex)
            {
                VirtualView?.SendLoadFailed(ex.Message);
            }
        }

        private class CustomMessageWebViewClient : MessageWebViewClient
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