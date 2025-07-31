using System;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using Foundation;
using Airship;
using CoreGraphics;
using WebKit;

namespace AirshipDotNet.Controls
{
    public partial class MessageViewHandler : ViewHandler<MessageView, UIView>
    {
        private UIView _containerView = null!;
        private WKWebView _webView = null!;
        private NSObject? _webViewObserver;

        protected override UIView CreatePlatformView()
        {
            _containerView = new UIView();
            _containerView.BackgroundColor = UIColor.SystemBackground;
            
            // Create web view with configuration
            var config = new WKWebViewConfiguration();
            config.WebsiteDataStore = WKWebsiteDataStore.DefaultDataStore;
            config.Preferences.JavaScriptEnabled = true;
            
            _webView = new WKWebView(_containerView.Bounds, config);
            _webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _webView.NavigationDelegate = new MessageWebViewDelegate(this);
            
            _containerView.AddSubview(_webView);
            
            return _containerView;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);
            
            if (VirtualView != null && !string.IsNullOrEmpty(VirtualView.MessageId))
            {
                LoadMessage(VirtualView.MessageId);
            }
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            if (_webViewObserver != null)
            {
                _webViewObserver.Dispose();
                _webViewObserver = null;
            }
            
            _webView?.RemoveFromSuperview();
            _webView?.Dispose();
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
            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                try
                {
                    VirtualView?.SendLoadStarted();

                    // Get message using wrapper
                    AWAirshipWrapper.GetMessageForID(messageId, (message) =>
                    {
                        if (message == null)
                        {
                            VirtualView?.SendLoadFailed("Message not found");
                            return;
                        }

                        NSRunLoop.Main.InvokeOnMainThread(() =>
                        {
                            // Mark as read
                            AWAirshipWrapper.MarkReadWithMessageIDs(new[] { messageId }, () => { });

                            // Get message URL
                            var bodyUrl = message.BodyURL;
                            if (bodyUrl != null)
                            {
                                // Get authentication credentials
                                AWAirshipWrapper.GetMessageCenterUserAuth((authString) =>
                                {
                                    NSRunLoop.Main.InvokeOnMainThread(() =>
                                    {
                                        if (authString != null)
                                        {
                                            // Create request with authentication header
                                            var mutableRequest = new NSMutableUrlRequest(bodyUrl);
                                            // The authString from basicAuthString already includes "Basic " prefix
                                            mutableRequest["Authorization"] = authString;
                                            _webView.LoadRequest(mutableRequest);
                                        }
                                        else
                                        {
                                            // Fallback to loading without auth if auth retrieval fails
                                            var request = new NSUrlRequest(bodyUrl);
                                            _webView.LoadRequest(request);
                                        }
                                    });
                                });
                            }
                            else
                            {
                                VirtualView?.SendLoadFailed("No message body URL available");
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    VirtualView?.SendLoadFailed(ex.Message);
                }
            });
        }

        private class MessageWebViewDelegate : NSObject, IWKNavigationDelegate
        {
            private readonly MessageViewHandler _handler;

            public MessageWebViewDelegate(MessageViewHandler handler)
            {
                _handler = handler;
            }

            [Export("webView:didFinishNavigation:")]
            public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                _handler.VirtualView?.SendLoadFinished();
            }

            [Export("webView:didFailNavigation:withError:")]
            public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                _handler.VirtualView?.SendLoadFailed(error.LocalizedDescription);
            }

            [Export("webView:didFailProvisionalNavigation:withError:")]
            public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                _handler.VirtualView?.SendLoadFailed(error.LocalizedDescription);
            }
        }
    }
}