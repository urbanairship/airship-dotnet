using Microsoft.Maui.Handlers;
using WebKit;
using CoreGraphics;
using UIKit;

using Foundation;
using UrbanAirship;
using AirshipDotNet.MessageCenter.Controls;
using Vision;

namespace AirshipDotNet.MessageCenter.Handlers;

/// <summary>
/// Handler responsible for displaying a single Message Center message via the platform WKWebView.
/// </summary>
[Preserve(AllMembers = true)]
public partial class MessageViewHandler : ViewHandler<IMessageView, WKWebView>
{
    public static PropertyMapper<IMessageView, MessageViewHandler> MessageViewMapper = new(ViewHandler.ViewMapper)
    {
        [nameof(IMessageView.MessageId)] = MapMessageId
    };

    private NativeBridgeDelegate nativeBridgeDelegate;
    private NavigationDelegate navigationDelegate;
    private UANativeBridge nativeBridge;
    private UAMessageCenterNativeBridgeExtension nativeBridgeExtension;
    private string messageId;
    private UAMessageCenterMessage message;
    private UAMessageCenterUser user;

    public MessageViewHandler() : base(MessageViewMapper)
    {
        UAMessageCenter.Shared.Inbox.GetUser(currentUser =>
        {
            UAMessageCenter.Shared.Inbox.MessageForID(messageId, currentMmessage =>
            {
                user = currentUser;
                message = currentMmessage;

                nativeBridgeDelegate = new(this);
                navigationDelegate = new(this);

                nativeBridge = new()
                {
                    ForwardNavigationDelegate = navigationDelegate,
                    NativeBridgeDelegate = nativeBridgeDelegate,
                    NativeBridgeExtensionDelegate = new UAMessageCenterNativeBridgeExtension(message, user)
                };

            });
        });
    }

    protected override WKWebView CreatePlatformView()
    {
        return new(UIScreen.MainScreen.Bounds, new WKWebViewConfiguration())
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            TranslatesAutoresizingMaskIntoConstraints = true,
            NavigationDelegate = nativeBridge
        };
    }

    private static void MapMessageId(MessageViewHandler handler, IMessageView entry)
    {
        if (entry.MessageId != null)
        {
            handler.LoadMessage(entry.MessageId, result =>
            {
            });
        }
    }

    public void LoadMessage(string messageId, Action<bool> result)
    {
        if (message != null)
        {
            LoadMessageBody(message, result);
        }
        else
        {
            UAMessageCenter.Shared.Inbox.RefreshMessages(refresh =>
            {
                if (refresh == true)
                {
                    UAMessageCenter.Shared.Inbox.MessageForID(messageId, newMessage =>
                    {
                        message = newMessage;
                        if (message != null && !message.IsExpired)
                        {
                            LoadMessageBody(message, result);
                        }
                        else
                        {
                            VirtualView.OnLoadFailed(messageId, false, MessageFailureStatus.Unavailable);
                            result(false);
                        }
                    });
                }
                else
                {
                    VirtualView.OnLoadFailed(messageId, false, MessageFailureStatus.FetchFailed);
                    result(false);
                }
            });
        }
    }

    protected void LoadMessageBody(UAMessageCenterMessage message, Action<bool> result)
    {
        if (user == null)
        {
            result(false);
        }

        var auth = UAUtils.AuthHeaderString(user.Username, user.Password);

        NSMutableDictionary dict = new NSMutableDictionary();
        dict.Add(new NSString("Authorization"), new NSString(auth));

        var request = new NSMutableUrlRequest(message.BodyURL);
        request.Headers = dict;

        MainThread.BeginInvokeOnMainThread(() =>
            PlatformView.LoadRequest(request)
        );

        VirtualView.OnLoadStarted(messageId);
        result(true);
    }

    private class NavigationDelegate : NSObject, IUANavigationDelegate
    {
        private MessageViewHandler Handler { get; set; }

        public NavigationDelegate(MessageViewHandler handler)
        {
            Handler = handler;
        }

        [Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            var response = navigationResponse.Response as NSHttpUrlResponse;
            if (response?.StatusCode >= 400 && response.StatusCode <= 599)
            {
                decisionHandler(WKNavigationResponsePolicy.Cancel);
                if (response.StatusCode == 410)
                {
                    Handler.VirtualView.OnLoadFailed(Handler.messageId, false, MessageFailureStatus.Unavailable);
                }
                else
                {
                    Handler.VirtualView.OnLoadFailed(Handler.messageId, true, MessageFailureStatus.LoadFailed);
                }
            }
            else
            {
                decisionHandler(WKNavigationResponsePolicy.Allow);
            }
        }

        [Export("webView:didFinishNavigation:")]
        public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            Handler.VirtualView.OnLoadFinished(Handler.messageId);
        }

        [Export("webView:didFailNavigation:withError:")]
        public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            Handler.VirtualView.OnLoadFailed(Handler.messageId, true, MessageFailureStatus.LoadFailed);
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            webView.NavigationDelegate?.DidFailNavigation(webView, navigation, error);
        }
    }

    private class NativeBridgeDelegate : NSObject, IUANativeBridgeDelegate
    {
        private MessageViewHandler Handler { get; set; }

        public NativeBridgeDelegate(MessageViewHandler handler)
        {
            Handler = handler;
        }

        public void Close()
        {
            Handler.VirtualView.OnClosed(Handler.messageId);
        }
    }
}