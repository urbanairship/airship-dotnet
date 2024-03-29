/*
 Copyright Airship and Contributors
*/
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using UIKit;
using UserNotifications;
using WebKit;

namespace UrbanAirship {

    // @interface UAMessageCenter
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenter
    {
        [Wrap("WeakDisplayDelegate")]
        [NullAllowed]
        IUAMessageCenterDisplayDelegate DisplayDelegate { get; set; }

        // @property (nonatomic, weak, readwrite) id<UAMessageCenterDisplayDelegate> _Nullable displayDelegate;
        [NullAllowed, Export("displayDelegate", ArgumentSemantic.Assign)]
        NSObject WeakDisplayDelegate { get; set; }

        // @property (nonatomic, strong) UAMessageCenterInbox * _Nonnull inbox;
        [Export("inbox", ArgumentSemantic.Strong)]
        UAMessageCenterInbox Inbox { get; set; }

        // -(void)setThemeFromPlist:(NSString * _Nonnull)plist error:(NSError * _Nonnull)error;
        [Export("setThemeFromPlist:error:")]
        void SetThemeFromPlist(string plist, NSError error);

        // @property (class, nonatomic, readonly, null_unspecified) UAMessageCenter *shared;
        [Static]
        [Export("shared")]
        UAMessageCenter Shared { get; }

        // -(void)display;
        [Export("display")]
        void Display();

        // -(void)displayWithMessageID:(NSString * _Nonnull)messageID;
        [Export("displayWithMessageID:")]
        void DisplayMessage(string messageID);

        // -(void)dismiss;
        [Export("dismiss")]
        void Dismiss();
    }

    // @interface UAMessageCenterController
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterController
    {
        // -(void)navigateWithMessageID:(NSString * _Nullable)messageID;
        [Export("navigateWithMessageID:")]
        void NavigateWithMessageID([NullAllowed] string messageID);
    }


    // @protocol UAMessageCenterDisplayDelegate
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterDisplayDelegate
    {
        // @required -(void)displayMessageCenterForMessageID:(NSString * _Nonnull)messageID;
        [Abstract]
        [Export("displayMessageCenterForMessageID:")]
        void OnDisplayMessageCenter(string messageID);

        // @required -(void)displayMessageCenter;
        [Abstract]
        [Export("displayMessageCenter")]
        void OnDisplayMessageCenter();

        // @required -(void)dismissMessageCenter;
        [Abstract]
        [Export("dismissMessageCenter")]
        void OnDismissMessageCenter();
    }

    interface IUAMessageCenterDisplayDelegate { }

    // @interface UAMessageCenterInbox
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterInbox
    {
        // -(void)getMessagesWithCompletionHandler:(void (^ _Nonnull)(int))completionHandler;
        [Export("getMessagesWithCompletionHandler:")]
        void GetMessages(Action<UAMessageCenterMessage[]> completionHandler);

        // -(void)getUserWithCompletionHandler:(void (^ _Nonnull)(UAMessageCenterUser * _Nullable))completionHandler;
        [Export("getUserWithCompletionHandler:")]
        void GetUser(Action<UAMessageCenterUser> completionHandler);

        // -(void)getUnreadCountWithCompletionHandler:(id)completionHandler;
        [Export("getUnreadCountWithCompletionHandler:")]
        void GetUnreadCount(Action<int> completionHandler);

        // -(void)refreshMessagesWithCompletionHandler:(Bool)completionHandler;
        [Export("refreshMessagesWithCompletionHandler:")]
        void RefreshMessages(Action<bool> completionHandler);

        // -(void)markReadWithMessages:(nonnull NSArray *)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Export("markReadWithMessages:completionHandler:")]
        void MarkReadWithMessages(UAMessageCenterMessage[] messages, Action completionHandler);

        // -(void)markReadWithMessageIDs:(nonnull NSArray *)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Export("markReadWithMessageIDs:completionHandler:")]
        void MarkReadWithMessageIDs(string[] messageIDs, Action completionHandler);

        // -(void)deleteWithMessages:(nonnull NSArray *)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Export("deleteWithMessages:completionHandler:")]
        void DeleteWithMessages(UAMessageCenterMessage[] messages, Action completionHandler);

        // -(void)deleteWithMessageIDs:(nonnull NSArray *)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Export("deleteWithMessageIDs:completionHandler:")]
        void DeleteWithMessageIDs(string[] messageIDs, Action completionHandler);

        // -(void)messageForBodyURL:(NSURL * _Nonnull)bodyURL completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
        [Export("messageForBodyURL:completionHandler:")]
        void MessageForBodyURL(NSUrl bodyURL, Action<UAMessageCenterMessage> completionHandler);

        // -(void)messageForID:(NSString * _Nonnull)messageID completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
        [Export("messageForID:completionHandler:")]
        void MessageForID(string messageID, Action<UAMessageCenterMessage> completionHandler);
    }

    // @protocol UAMessageCenterInboxBaseProtocol
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterInboxBaseProtocol
    {
        // @required -(void)getMessagesWithCompletionHandler:(void (^ _Nonnull)(int))completionHandler;
        [Abstract]
        [Export("getMessagesWithCompletionHandler:")]
        void GetMessages(Action<UAMessageCenterMessage[]> completionHandler);

        // @required -(void)getUserWithCompletionHandler:(void (^ _Nonnull)(UAMessageCenterUser * _Nullable))completionHandler;
        [Abstract]
        [Export("getUserWithCompletionHandler:")]
        void GetUser(Action<UAMessageCenterUser> completionHandler);

        // @required -(void)getUnreadCountWithCompletionHandler:(id)completionHandler;
        [Abstract]
        [Export("getUnreadCountWithCompletionHandler:")]
        void GetUnreadCount(Action<int> completionHandler);

        // @required -(void)refreshMessagesWithCompletionHandler:(Bool)completionHandler;
        [Abstract]
        [Export("refreshMessagesWithCompletionHandler:")]
        void RefreshMessages(Action<bool> completionHandler);

        // @required -(void)markReadWithMessages:(nonnull NSArray *)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Abstract]
        [Export("markReadWithMessages:completionHandler:")]
        void MarkRead(UAMessageCenterMessage[] messages, Action completionHandler);

        // @required -(void)markReadWithMessageIDs:(nonnull NSArray *)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Abstract]
        [Export("markReadWithMessageIDs:completionHandler:")]
        void MarkRead(string[] messageIDs, Action completionHandler);

        // @required -(void)deleteWithMessages:(nonnull NSArray *)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Abstract]
        [Export("deleteWithMessages:completionHandler:")]
        void Delete(UAMessageCenterMessage[] messages, Action completionHandler);

        // @required -(void)deleteWithMessageIDs:(nonnull NSArray *)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
        [Abstract]
        [Export("deleteWithMessageIDs:completionHandler:")]
        void Delete(string[] messageIDs, Action completionHandler);

        // @required -(void)messageForBodyURL:(NSURL * _Nonnull)bodyURL completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
        [Abstract]
        [Export("messageForBodyURL:completionHandler:")]
        void MessageForBodyURL(NSUrl bodyURL, Action<UAMessageCenterMessage> completionHandler);

        // @required -(void)messageForID:(NSString * _Nonnull)messageID completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
        [Abstract]
        [Export("messageForID:completionHandler:")]
        void MessageForID(string messageID, Action<UAMessageCenterMessage> completionHandler);
    }

    // @interface UAMessageCenterMessage
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterMessage
    {
        // @property (readonly, copy, nonatomic) NSString * _Nonnull title;
        [Export("title")]
        string Title { get; }

        // @property (readonly, copy, nonatomic) NSString * _Nonnull id;
        [Export("id")]
        string Id { get; }

        // @property (nonatomic, readonly) NSDictionary *_Nonnull extra;
        [Export("extra")]
        NSDictionary Extra { get; }

        // @property (readonly, copy, nonatomic) NSURL * _Nonnull bodyURL;
        [Export("bodyURL", ArgumentSemantic.Copy)]
        NSUrl BodyURL { get; }

        // @property (readonly, copy, nonatomic) NSDate * _Nullable expirationDate;
        [NullAllowed, Export("expirationDate", ArgumentSemantic.Copy)]
        NSDate ExpirationDate { get; }

        // @property (readonly, copy, nonatomic) NSDate * _Nonnull sentDate;
        [Export("sentDate", ArgumentSemantic.Copy)]
        NSDate SentDate { get; }

        // @property (readonly, nonatomic) Bool unread;
        [Export("unread")]
        bool Unread { get; }

        // @property (readonly, copy, nonatomic) NSString * _Nullable listIcon;
        [NullAllowed, Export("listIcon")]
        string ListIcon { get; }

        // @property (readonly, nonatomic) Bool isExpired;
        [Export("isExpired")]
        bool IsExpired { get; }
    }

    // @interface UAMessageCenterNativeBridgeExtension : NSObject <UANativeBridgeExtensionDelegate>
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterNativeBridgeExtension : IUANativeBridgeExtensionDelegate
    {
        // -(instancetype _Nonnull)initWithMessage:(UAMessageCenterMessage * _Nonnull)message user:(UAMessageCenterUser * _Nonnull)user __attribute__((objc_designated_initializer));
        [Export("initWithMessage:user:")]
        [DesignatedInitializer]
        IntPtr Constructor(UAMessageCenterMessage message, UAMessageCenterUser user);
    }

    // @interface UAMessageCenterUser
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterUser
    {
        // @property (readonly, copy, nonatomic) NSString * _Nonnull password;
        [Export("password")]
        string Password { get; }

        // @property (readonly, copy, nonatomic) NSString * _Nonnull username;
        [Export("username")]
        string Username { get; }
    }

    // @interface UAMessageCenterViewController
    [BaseType(typeof(NSObject))]
    interface UAMessageCenterViewController
    {
        // +(UIViewController * _Nullable)makeWithThemePlist:(NSString * _Nullable)themePlist controller:(UAMessageCenterController * _Nonnull)controller error:(NSError * _Nonnull)error dismissAction:(void (^ _Nullable)(void))dismissAction __attribute__((warn_unused_result("")));
        [Static]
        [Export("makeWithThemePlist:controller:error:dismissAction:")]
        [return: NullAllowed]
        UIViewController MakeWithThemePlist([NullAllowed] string themePlist, UAMessageCenterController controller, NSError error, [NullAllowed] Action dismissAction);
    }
}
