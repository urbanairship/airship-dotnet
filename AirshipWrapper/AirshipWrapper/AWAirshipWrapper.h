#import <Foundation/Foundation.h>

// Forward declarations for all wrapped types
@class UAChannel;
@class UAContact;
@class UAPush;
@class UAMessageCenter;
@class UAMessageCenterMessage;
@class UAInAppAutomation;
@class UAAnalytics;
@class UAPrivacyManager;
@class UAPreferenceCenter;
@class NSError;

NS_ASSUME_NONNULL_BEGIN

@interface AWAirshipWrapper : NSObject

// Shared instance access
+ (instancetype)shared;

#pragma mark - Core Components (wrapped properties)

@property (nonatomic, readonly) UAChannel *channel;
@property (nonatomic, readonly) UAContact *contact;
@property (nonatomic, readonly) UAPush *push;
@property (nonatomic, readonly) UAMessageCenter *messageCenter;
@property (nonatomic, readonly) UAInAppAutomation *inAppAutomation;
@property (nonatomic, readonly) UAAnalytics *analytics;
@property (nonatomic, readonly) UAPrivacyManager *privacyManager;
@property (nonatomic, readonly) UAPreferenceCenter *preferenceCenter;

#pragma mark - Method Wrappers

+ (void)getMessages:(void(^)(NSArray<UAMessageCenterMessage *> *))completion;
+ (void)getNamedUserID:(void(^)(NSString * _Nullable, NSError * _Nullable))completion;
+ (void)fetchChannelSubscriptionLists:(void(^)(NSArray<NSString *> * _Nullable, NSError * _Nullable))completion;
+ (void)fetchContactSubscriptionLists:(void(^)(NSDictionary<NSString *, NSArray<NSString *> *> * _Nullable, NSError * _Nullable))completion;
+ (void)getMessageCenterUserAuth:(void(^)(NSString * _Nullable))completion;
+ (void)getMessageForID:(NSString *)messageID completion:(void(^)(UAMessageCenterMessage * _Nullable))completion;
+ (void)markReadWithMessageIDs:(NSArray<NSString *> *)messageIDs completion:(void(^)(void))completion;
+ (void)resetBadgeWithCompletion:(void(^)(NSError * _Nullable))completion;

@end

NS_ASSUME_NONNULL_END