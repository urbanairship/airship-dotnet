#import "AWAirshipWrapper.h"
#import <AirshipObjectiveC/AirshipObjectiveC-Swift.h>
#import <objc/runtime.h>

@interface AWAirshipWrapper ()
@end

@implementation AWAirshipWrapper

static AWAirshipWrapper *_shared = nil;

#pragma mark - Initialization

+ (instancetype)shared {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _shared = [[AWAirshipWrapper alloc] init];
    });
    return _shared;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        // Initialization if needed
    }
    return self;
}

#pragma mark - Core Components

- (UAChannel *)channel {
    return [UAirship channel];
}

- (UAContact *)contact {
    return [UAirship contact];
}


- (UAPush *)push {
    return [UAirship push];
}

- (UAMessageCenter *)messageCenter {
    return [UAirship messageCenter];
}

- (UAInAppAutomation *)inAppAutomation {
    return [UAirship inAppAutomation];
}

- (UAAnalytics *)analytics {
    return [UAirship analytics];
}

- (UAPrivacyManager *)privacyManager {
    return [UAirship privacyManager];
}

- (UAPreferenceCenter *)preferenceCenter {
    return [UAirship preferenceCenter];
}

#pragma mark - Method Wrappers

+ (void)getMessages:(void(^)(NSArray<UAMessageCenterMessage *> *))completion {
    [[UAirship messageCenter].inbox getMessagesWithCompletionHandler:completion];
}

+ (void)getNamedUserID:(void(^)(NSString * _Nullable, NSError * _Nullable))completion {
    [[UAirship contact] getNamedUserIDWithCompletionHandler:completion];
}

+ (void)fetchChannelSubscriptionLists:(void(^)(NSArray<NSString *> * _Nullable, NSError * _Nullable))completion {
    [[UAirship channel] fetchSubscriptionListsWithCompletionHandler:completion];
}

+ (void)fetchContactSubscriptionLists:(void(^)(NSDictionary<NSString *, NSArray<NSString *> *> * _Nullable, NSError * _Nullable))completion {
    [[UAirship contact] fetchSubscriptionListsWithCompletionHandler:completion];
}

+ (void)getMessageCenterUserAuth:(void(^)(NSString * _Nullable))completion {
    // Directly extract the auth string without creating intermediate objects
    [[UAirship messageCenter].inbox getUserWithCompletionHandler:^(id _Nullable userObject) {
        if (userObject) {
            @try {
                SEL basicAuthSelector = @selector(basicAuthString);
                if ([userObject respondsToSelector:basicAuthSelector]) {
                    NSString *authString = [userObject performSelector:basicAuthSelector];
                    completion(authString);
                } else {
                    NSLog(@"UAMessageCenterUser doesn't respond to basicAuthString selector");
                    completion(nil);
                }
            } @catch (NSException *exception) {
                NSLog(@"Failed to extract auth string: %@", exception);
                completion(nil);
            }
        } else {
            completion(nil);
        }
    }];
}

+ (void)getMessageForID:(NSString *)messageID completion:(void(^)(UAMessageCenterMessage * _Nullable))completion {
    [[UAirship messageCenter].inbox messageForID:messageID completionHandler:completion];
}

+ (void)markReadWithMessageIDs:(NSArray<NSString *> *)messageIDs completion:(void(^)(void))completion {
    [[UAirship messageCenter].inbox markReadWithMessageIDs:messageIDs completionHandler:completion];
}

+ (void)resetBadgeWithCompletion:(void(^)(NSError * _Nullable))completion {
    [[UAirship push] resetBadgeWithCompletionHandler:completion];
}

@end