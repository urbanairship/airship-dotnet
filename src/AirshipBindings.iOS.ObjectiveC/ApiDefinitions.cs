using System;

using Foundation;
using ObjCRuntime;
using UIKit;
using UserNotifications;

namespace Airship
{
	// AWAirshipWrapper provides a workaround for Swift async marshaling issues
	// by wrapping these 4 problematic methods in Objective-C completion handlers
	// @interface AWAirshipWrapper : NSObject
	[BaseType (typeof(NSObject))]
	interface AWAirshipWrapper
	{
		// + (instancetype)shared;
		[Static]
		[Export ("shared")]
		AWAirshipWrapper Shared { get; }

		// @property (nonatomic, readonly) UAChannel *channel;
		[Export ("channel")]
		UAChannel Channel { get; }

		// @property (nonatomic, readonly) UAContact *contact;
		[Export ("contact")]
		UAContact Contact { get; }

		// @property (nonatomic, readonly) UAPush *push;
		[Export ("push")]
		UAPush Push { get; }

		// @property (nonatomic, readonly) UAMessageCenter *messageCenter;
		[Export ("messageCenter")]
		UAMessageCenter MessageCenter { get; }

		// @property (nonatomic, readonly) UAInAppAutomation *inAppAutomation;
		[Export ("inAppAutomation")]
		UAInAppAutomation InAppAutomation { get; }

		// @property (nonatomic, readonly) UAAnalytics *analytics;
		[Export ("analytics")]
		UAAnalytics Analytics { get; }

		// @property (nonatomic, readonly) UAPrivacyManager *privacyManager;
		[Export ("privacyManager")]
		UAPrivacyManager PrivacyManager { get; }

		// @property (nonatomic, readonly) UAPreferenceCenter *preferenceCenter;
		[Export ("preferenceCenter")]
		UAPreferenceCenter PreferenceCenter { get; }

		// @property (nonatomic, readonly) UAPermissionsManager *permissionsManager;
		[Export ("permissionsManager")]
		UAPermissionsManager PermissionsManager { get; }

		// + (void)getMessages:(void(^)(NSArray<UAMessageCenterMessage *> *))completion;
		[Static]
		[Export ("getMessages:")]
		void GetMessages (Action<UAMessageCenterMessage[]> completion);

		// + (void)getNamedUserID:(void(^)(NSString * _Nullable, NSError * _Nullable))completion;
		[Static]
		[Export ("getNamedUserID:")]
		void GetNamedUserID (Action<string, NSError> completion);

		// + (void)fetchChannelSubscriptionLists:(void(^)(NSArray<NSString *> * _Nullable, NSError * _Nullable))completion;
		[Static]
		[Export ("fetchChannelSubscriptionLists:")]
		void FetchChannelSubscriptionLists (Action<NSArray, NSError> completion);

		// + (void)fetchContactSubscriptionLists:(void(^)(NSDictionary<NSString *, NSArray<NSString *> *> * _Nullable, NSError * _Nullable))completion;
		[Static]
		[Export ("fetchContactSubscriptionLists:")]
		void FetchContactSubscriptionLists (Action<NSDictionary<NSString, NSArray>, NSError> completion);

		// + (void)getMessageCenterUserAuth:(void(^)(NSString * _Nullable))completion;
		[Static]
		[Export ("getMessageCenterUserAuth:")]
		void GetMessageCenterUserAuth (Action<NSString> completion);

		// + (void)getMessageForID:(NSString *)messageID completion:(void(^)(UAMessageCenterMessage * _Nullable))completion;
		[Static]
		[Export ("getMessageForID:completion:")]
		void GetMessageForID (string messageID, Action<UAMessageCenterMessage> completion);

		// + (void)markReadWithMessageIDs:(NSArray<NSString *> *)messageIDs completion:(void(^)(void))completion;
		[Static]
		[Export ("markReadWithMessageIDs:completion:")]
		void MarkReadWithMessageIDs (string[] messageIDs, Action completion);

		// + (void)resetBadgeWithCompletion:(void(^)(NSError * _Nullable))completion;
		[Static]
		[Export ("resetBadgeWithCompletion:")]
		void ResetBadgeWithCompletion (Action<NSError> completion);
	}

	// @interface UAAirshipNotifications : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC22UAAirshipNotifications")]
	interface UAAirshipNotifications
	{
	}

	// @interface UAAirshipNotificationsAirshipReady : NSObject
	[BaseType (typeof(NSObject))]
	interface UAAirshipNotificationsAirshipReady
	{
		// @property (readonly, nonatomic, class) NSNotificationName _Nonnull name;
		[Static]
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull channelIDKey;
		[Static]
		[Export ("channelIDKey")]
		string ChannelIDKey { get; }

		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull appKey;
		[Static]
		[Export ("appKey")]
		string AppKey { get; }

		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull payloadVersionKey;
		[Static]
		[Export ("payloadVersionKey")]
		string PayloadVersionKey { get; }
	}

	// @interface UAirshipNotificationChannelCreated : NSObject
	[BaseType (typeof(NSObject))]
	interface UAirshipNotificationChannelCreated
	{
		// @property (readonly, nonatomic, class) NSNotificationName _Nonnull name;
		[Static]
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull channelIDKey;
		[Static]
		[Export ("channelIDKey")]
		string ChannelIDKey { get; }

		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull isExistingChannelKey;
		[Static]
		[Export ("isExistingChannelKey")]
		string IsExistingChannelKey { get; }
	}


	// @interface UAAirshipNotificationReceivedNotificationResponse : NSObject
	[BaseType (typeof(NSObject))]
	interface UAAirshipNotificationReceivedNotificationResponse
	{
		// @property (readonly, nonatomic, class) NSNotificationName _Nonnull name;
		[Static]
		[Export ("name")]
		string Name { get; }
	}

	// @interface UAAirshipNotificationRecievedNotification : NSObject
	[BaseType (typeof(NSObject))]
	interface UAAirshipNotificationRecievedNotification
	{
		// @property (readonly, nonatomic, class) NSNotificationName _Nonnull name;
		[Static]
		[Export ("name")]
		string Name { get; }
	}

	// @interface UAAnalytics : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC11UAAnalytics")]
	[DisableDefaultCtor]
	interface UAAnalytics
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull sessionID;
		[Export ("sessionID")]
		string SessionID { get; }

		// -(void)trackScreen:(NSString * _Nullable)screen;
		[Export ("trackScreen:")]
		void TrackScreen ([NullAllowed] string screen);

		// -(void)recordCustomEvent:(UACustomEvent * _Nonnull)event;
		[Export ("recordCustomEvent:")]
		void RecordCustomEvent (UACustomEvent @event);

		// -(void)associateDeviceIdentifier:(UAAssociatedIdentifiers * _Nonnull)associatedIdentifiers;
		[Export ("associateDeviceIdentifier:")]
		void AssociateDeviceIdentifier (UAAssociatedIdentifiers associatedIdentifiers);

		// -(UAAssociatedIdentifiers * _Nonnull)currentAssociatedDeviceIdentifiers __attribute__((warn_unused_result("")));
		[Export ("currentAssociatedDeviceIdentifiers")]
		UAAssociatedIdentifiers CurrentAssociatedDeviceIdentifiers { get; }
	}

	// @interface UAAppIntegration : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC16UAAppIntegration")]
	interface UAAppIntegration
	{
		// +(void)application:(UIApplication * _Nonnull)application performFetchWithCompletionHandler:(void (^ _Nonnull)(UIBackgroundFetchResult))completionHandler;
		[Static]
		[Export ("application:performFetchWithCompletionHandler:")]
		void Application (UIApplication application, Action<UIBackgroundFetchResult> completionHandler);

		// +(void)application:(UIApplication * _Nonnull)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData * _Nonnull)deviceToken;
		[Static]
		[Export ("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
		void Application (UIApplication application, NSData deviceToken);

		// +(void)application:(UIApplication * _Nonnull)application didFailToRegisterForRemoteNotificationsWithError:(NSError * _Nonnull)error;
		[Static]
		[Export ("application:didFailToRegisterForRemoteNotificationsWithError:")]
		void Application (UIApplication application, NSError error);

		// +(void)application:(UIApplication * _Nonnull)application didReceiveRemoteNotification:(NSDictionary * _Nonnull)userInfo fetchCompletionHandler:(void (^ _Nonnull)(UIBackgroundFetchResult))completionHandler;
		[Static]
		[Export ("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
		void Application (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler);

		// +(void)userNotificationCenter:(UNUserNotificationCenter * _Nonnull)center willPresentNotification:(UNNotification * _Nonnull)notification withCompletionHandler:(void (^ _Nonnull)(UNNotificationPresentationOptions))completionHandler;
		[Static]
		[Export ("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
		void UserNotificationCenter (UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler);

		// +(void)userNotificationCenter:(UNUserNotificationCenter * _Nonnull)center didReceiveNotificationResponse:(UNNotificationResponse * _Nonnull)response withCompletionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Static]
		[Export ("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
		void UserNotificationCenter (UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler);
	}

	// @interface UAAssociatedIdentifiers : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC23UAAssociatedIdentifiers")]
	[DisableDefaultCtor]
	interface UAAssociatedIdentifiers
	{
		// -(void)setWithIdentifier:(NSString * _Nullable)identifier key:(NSString * _Nonnull)key;
		[Export ("setWithIdentifier:key:")]
		void SetWithIdentifier ([NullAllowed] string identifier, string key);
	}

	// @interface UAAttributesEditor : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC18UAAttributesEditor")]
	interface UAAttributesEditor
	{
		// -(void)removeAttribute:(NSString * _Nonnull)attribute;
		[Export ("removeAttribute:")]
		void RemoveAttribute (string attribute);

		// -(void)setDate:(NSDate * _Nonnull)date attribute:(NSString * _Nonnull)attribute;
		[Export ("setDate:attribute:")]
		void SetDate (NSDate date, string attribute);

		// -(void)setNumber:(NSNumber * _Nonnull)number attribute:(NSString * _Nonnull)attribute;
		[Export ("setNumber:attribute:")]
		void SetNumber (NSNumber number, string attribute);

		// -(void)setString:(NSString * _Nonnull)string attribute:(NSString * _Nonnull)attribute;
		[Export ("setString:attribute:")]
		void SetString (string @string, string attribute);

		// -(void)apply;
		[Export ("apply")]
		void Apply ();
	}

	// @interface UAAuthorizedNotificationSettings : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC32UAAuthorizedNotificationSettings")]
	[DisableDefaultCtor]
	interface UAAuthorizedNotificationSettings
	{
		// +(UAAuthorizedNotificationSettings * _Nonnull)badge __attribute__((warn_unused_result("")));
		[Static]
		[Export ("badge")]
		UAAuthorizedNotificationSettings Badge { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)sound __attribute__((warn_unused_result("")));
		[Static]
		[Export ("sound")]
		UAAuthorizedNotificationSettings Sound { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)alert __attribute__((warn_unused_result("")));
		[Static]
		[Export ("alert")]
		UAAuthorizedNotificationSettings Alert { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)carPlay __attribute__((warn_unused_result("")));
		[Static]
		[Export ("carPlay")]
		UAAuthorizedNotificationSettings CarPlay { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)lockScreen __attribute__((warn_unused_result("")));
		[Static]
		[Export ("lockScreen")]
		UAAuthorizedNotificationSettings LockScreen { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)notificationCenter __attribute__((warn_unused_result("")));
		[Static]
		[Export ("notificationCenter")]
		UAAuthorizedNotificationSettings NotificationCenter { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)criticalAlert __attribute__((warn_unused_result("")));
		[Static]
		[Export ("criticalAlert")]
		UAAuthorizedNotificationSettings CriticalAlert { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)announcement __attribute__((warn_unused_result("")));
		[Static]
		[Export ("announcement")]
		UAAuthorizedNotificationSettings Announcement { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)scheduledDelivery __attribute__((warn_unused_result("")));
		[Static]
		[Export ("scheduledDelivery")]
		UAAuthorizedNotificationSettings ScheduledDelivery { get; }

		// +(UAAuthorizedNotificationSettings * _Nonnull)timeSensitive __attribute__((warn_unused_result("")));
		[Static]
		[Export ("timeSensitive")]
		UAAuthorizedNotificationSettings TimeSensitive { get; }

		// @property (readonly, nonatomic) NSUInteger hash;
		[Export ("hash")]
		nuint Hash { get; }

		// -(BOOL)isEqual:(id _Nullable)object __attribute__((warn_unused_result("")));
		[Override]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject @object);
	}

	// @interface UAChannel : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC9UAChannel")]
	[DisableDefaultCtor]
	interface UAChannel
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable identifier;
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		// @property (copy, nonatomic) NSArray<NSString *> * _Nonnull tags;
		[Export ("tags", ArgumentSemantic.Copy)]
		string[] Tags { get; set; }

		// -(UATagEditor * _Nullable)editTags __attribute__((warn_unused_result("")));
		[NullAllowed, Export ("editTags")]
		UATagEditor EditTags { get; }

		// -(UATagGroupsEditor * _Nonnull)editTagGroups __attribute__((warn_unused_result("")));
		[Export ("editTagGroups")]
		UATagGroupsEditor EditTagGroups { get; }

		// -(void)editTagGroups:(void (^ _Nonnull)(UATagGroupsEditor * _Nonnull))editorBlock;
		[Export ("editTagGroups:")]
		void EditTagGroupsWithBlock (Action<UATagGroupsEditor> editorBlock);

		// -(UASubscriptionListEditor * _Nonnull)editSubscriptionLists __attribute__((warn_unused_result("")));
		[Export ("editSubscriptionLists")]
		UASubscriptionListEditor EditSubscriptionLists { get; }

		// -(void)editSubscriptionLists:(void (^ _Nonnull)(UASubscriptionListEditor * _Nonnull))editorBlock;
		[Export ("editSubscriptionLists:")]
		void EditSubscriptionListsWithBlock (Action<UASubscriptionListEditor> editorBlock);

		// -(void)fetchSubscriptionListsWithCompletionHandler:(void (^ _Nonnull)(NSArray<NSString *> * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("fetchSubscriptionListsWithCompletionHandler:")]
		void FetchSubscriptionListsWithCompletionHandler (Action<NSArray, NSError> completionHandler);

		// -(void)fetchSubscriptionListsWithCompletion:(void (^ _Nonnull)(NSArray<NSString *> * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("fetchSubscriptionListsWithCompletion:")]
		void FetchSubscriptionListsWithCompletion (Action<NSArray, NSError> completionHandler);

		// -(UAAttributesEditor * _Nonnull)editAttributes __attribute__((warn_unused_result("")));
		[Export ("editAttributes")]
		UAAttributesEditor EditAttributes { get; }

		// -(void)editAttributes:(void (^ _Nonnull)(UAAttributesEditor * _Nonnull))editorBlock;
		[Export ("editAttributes:")]
		void EditAttributesWithBlock (Action<UAAttributesEditor> editorBlock);

		// -(void)enableChannelCreation;
		[Export ("enableChannelCreation")]
		void EnableChannelCreation ();
	}

	// @interface UAConfig : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC8UAConfig")]
	[DisableDefaultCtor]
	interface UAConfig
	{
		// @property (copy, nonatomic) NSString * _Nullable developmentAppKey;
		[NullAllowed, Export ("developmentAppKey")]
		string DevelopmentAppKey { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable developmentAppSecret;
		[NullAllowed, Export ("developmentAppSecret")]
		string DevelopmentAppSecret { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable productionAppKey;
		[NullAllowed, Export ("productionAppKey")]
		string ProductionAppKey { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable productionAppSecret;
		[NullAllowed, Export ("productionAppSecret")]
		string ProductionAppSecret { get; set; }

		// @property (nonatomic) enum UAAirshipLogLevel developmentLogLevel;
		[Export ("developmentLogLevel", ArgumentSemantic.Assign)]
		UAAirshipLogLevel DevelopmentLogLevel { get; set; }

		// @property (nonatomic) enum UAAirshipLogLevel productionLogLevel;
		[Export ("productionLogLevel", ArgumentSemantic.Assign)]
		UAAirshipLogLevel ProductionLogLevel { get; set; }

		// @property (nonatomic) BOOL autoPauseInAppAutomationOnLaunch;
		[Export ("autoPauseInAppAutomationOnLaunch")]
		bool AutoPauseInAppAutomationOnLaunch { get; set; }

		// @property (nonatomic) enum UACloudSite site;
		[Export ("site", ArgumentSemantic.Assign)]
		UACloudSite Site { get; set; }

		// @property (nonatomic, strong) UAFeature * _Nonnull enabledFeatures;
		[Export ("enabledFeatures", ArgumentSemantic.Strong)]
		UAFeature EnabledFeatures { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable defaultAppKey;
		[NullAllowed, Export ("defaultAppKey")]
		string DefaultAppKey { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable defaultAppSecret;
		[NullAllowed, Export ("defaultAppSecret")]
		string DefaultAppSecret { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable inProduction;
		[NullAllowed, Export ("inProduction", ArgumentSemantic.Strong)]
		NSNumber InProduction { get; set; }

		// @property (nonatomic) BOOL isAutomaticSetupEnabled;
		[Export ("isAutomaticSetupEnabled")]
		bool IsAutomaticSetupEnabled { get; set; }

		// @property (copy, nonatomic) NSArray<NSString *> * _Nullable URLAllowList;
		[NullAllowed, Export ("URLAllowList", ArgumentSemantic.Copy)]
		string[] URLAllowList { get; set; }

		// @property (copy, nonatomic) NSArray<NSString *> * _Nullable URLAllowListScopeJavaScriptInterface;
		[NullAllowed, Export ("URLAllowListScopeJavaScriptInterface", ArgumentSemantic.Copy)]
		string[] URLAllowListScopeJavaScriptInterface { get; set; }

		// @property (copy, nonatomic) NSArray<NSString *> * _Nullable URLAllowListScopeOpenURL;
		[NullAllowed, Export ("URLAllowListScopeOpenURL", ArgumentSemantic.Copy)]
		string[] URLAllowListScopeOpenURL { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable itunesID;
		[NullAllowed, Export ("itunesID")]
		string ItunesID { get; set; }

		// @property (nonatomic) BOOL isAnalyticsEnabled;
		[Export ("isAnalyticsEnabled")]
		bool IsAnalyticsEnabled { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable messageCenterStyleConfig;
		[NullAllowed, Export ("messageCenterStyleConfig")]
		string MessageCenterStyleConfig { get; set; }

		// @property (nonatomic) BOOL clearUserOnAppRestore;
		[Export ("clearUserOnAppRestore")]
		bool ClearUserOnAppRestore { get; set; }

		// @property (nonatomic) BOOL clearNamedUserOnAppRestore;
		[Export ("clearNamedUserOnAppRestore")]
		bool ClearNamedUserOnAppRestore { get; set; }

		// @property (nonatomic) BOOL isChannelCaptureEnabled;
		[Export ("isChannelCaptureEnabled")]
		bool IsChannelCaptureEnabled { get; set; }

		// @property (nonatomic) BOOL isChannelCreationDelayEnabled;
		[Export ("isChannelCreationDelayEnabled")]
		bool IsChannelCreationDelayEnabled { get; set; }

		// @property (nonatomic) BOOL isExtendedBroadcastsEnabled;
		[Export ("isExtendedBroadcastsEnabled")]
		bool IsExtendedBroadcastsEnabled { get; set; }

		// @property (nonatomic) BOOL requestAuthorizationToUseNotifications;
		[Export ("requestAuthorizationToUseNotifications")]
		bool RequestAuthorizationToUseNotifications { get; set; }

		// @property (nonatomic) BOOL requireInitialRemoteConfigEnabled;
		[Export ("requireInitialRemoteConfigEnabled")]
		bool RequireInitialRemoteConfigEnabled { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable initialConfigURL;
		[NullAllowed, Export ("initialConfigURL")]
		string InitialConfigURL { get; set; }

		// @property (nonatomic) BOOL useUserPreferredLocale;
		[Export ("useUserPreferredLocale")]
		bool UseUserPreferredLocale { get; set; }

		// +(UAConfig * _Nullable)defaultConfigWithError:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("defaultConfigWithError:")]
		[return: NullAllowed]
		UAConfig DefaultConfigWithError ([NullAllowed] out NSError error);

		// +(UAConfig * _Nullable)fromPlistWithContentsOfFile:(NSString * _Nonnull)path error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("fromPlistWithContentsOfFile:error:")]
		[return: NullAllowed]
		UAConfig FromPlistWithContentsOfFile (string path, [NullAllowed] out NSError error);

		// +(UAConfig * _Nonnull)config __attribute__((warn_unused_result("")));
		[Static]
		[Export ("config")]
		UAConfig Config { get; }

		// +(UAConfig * _Nullable)defaultConfigAndReturnError:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("defaultConfigAndReturnError:")]
		[return: NullAllowed]
		UAConfig DefaultConfigAndReturnError ([NullAllowed] out NSError error);

		// -(BOOL)validateCredentialsInProduction:(BOOL)inProduction error:(NSError * _Nullable * _Nullable)error;
		[Export ("validateCredentialsInProduction:error:")]
		bool ValidateCredentialsInProduction (bool inProduction, [NullAllowed] out NSError error);
	}

	// @interface UAContact : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC9UAContact")]
	[DisableDefaultCtor]
	interface UAContact
	{
		// -(void)identify:(NSString * _Nonnull)namedUserID;
		[Export ("identify:")]
		void Identify (string namedUserID);

		// -(void)reset;
		[Export ("reset")]
		void Reset ();

		// -(void)notifyRemoteLogin;
		[Export ("notifyRemoteLogin")]
		void NotifyRemoteLogin ();

		// -(UATagGroupsEditor * _Nonnull)editTagGroups __attribute__((warn_unused_result("")));
		[Export ("editTagGroups")]
		UATagGroupsEditor EditTagGroups { get; }

		// -(void)editTagGroups:(void (^ _Nonnull)(UATagGroupsEditor * _Nonnull))editorBlock;
		[Export ("editTagGroups:")]
		void EditTagGroupsWithBlock (Action<UATagGroupsEditor> editorBlock);

		// -(UAAttributesEditor * _Nonnull)editAttributes __attribute__((warn_unused_result("")));
		[Export ("editAttributes")]
		UAAttributesEditor EditAttributes { get; }

		// -(void)editAttributes:(void (^ _Nonnull)(UAAttributesEditor * _Nonnull))editorBlock;
		[Export ("editAttributes:")]
		void EditAttributesWithBlock (Action<UAAttributesEditor> editorBlock);

		// -(void)associateChannel:(NSString * _Nonnull)channelID type:(enum UAChannelType)type;
		[Export ("associateChannel:type:")]
		void AssociateChannel (string channelID, UAChannelType type);

		// -(UAScopedSubscriptionListEditor * _Nonnull)editSubscriptionLists __attribute__((warn_unused_result("")));
		[Export ("editSubscriptionLists")]
		UAScopedSubscriptionListEditor EditSubscriptionLists { get; }

		// -(void)editSubscriptionLists:(void (^ _Nonnull)(UAScopedSubscriptionListEditor * _Nonnull))editorBlock;
		[Export ("editSubscriptionLists:")]
		void EditSubscriptionListsWithBlock (Action<UAScopedSubscriptionListEditor> editorBlock);

		// -(void)fetchSubscriptionListsWithCompletionHandler:(void (^ _Nonnull)(NSDictionary<NSString *,NSArray<NSString *> *> * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("fetchSubscriptionListsWithCompletionHandler:")]
		void FetchSubscriptionListsWithCompletionHandler (Action<NSDictionary, NSError> completionHandler);

		// -(void)getNamedUserIDWithCompletionHandler:(void (^ _Nonnull)(NSString * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("getNamedUserIDWithCompletionHandler:")]
		void GetNamedUserIDWithCompletionHandler (Action<NSString, NSError> completionHandler);
	}

	// @interface UACustomEvent : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC13UACustomEvent")]
	[DisableDefaultCtor]
	interface UACustomEvent
	{
		// @property (nonatomic) NSDecimal eventValue;
		[Export ("eventValue", ArgumentSemantic.Assign)]
		NSDecimal EventValue { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull eventName;
		[Export ("eventName")]
		string EventName { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable transactionID;
		[NullAllowed, Export ("transactionID")]
		string TransactionID { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable interactionType;
		[NullAllowed, Export ("interactionType")]
		string InteractionType { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable interactionID;
		[NullAllowed, Export ("interactionID")]
		string InteractionID { get; set; }

		// @property (readonly, copy, nonatomic) NSDictionary<NSString *,id> * _Nonnull properties;
		[Export ("properties", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Properties { get; }

		// -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name;
		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		// -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name value:(double)value;
		[Export ("initWithName:value:")]
		NativeHandle Constructor (string name, double value);

		// -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name decimalValue:(NSDecimal)decimalValue;
		[Export ("initWithName:decimalValue:")]
		NativeHandle Constructor (string name, NSDecimal decimalValue);

		// -(BOOL)isValid __attribute__((warn_unused_result("")));
		[Export ("isValid")]
		bool IsValid { get; }

		// -(void)track;
		[Export ("track")]
		void Track ();

		// -(void)setPropertyWithString:(NSString * _Nonnull)string forKey:(NSString * _Nonnull)key;
		[Export ("setPropertyWithString:forKey:")]
		void SetPropertyWithString (string @string, string key);

		// -(void)removePropertyForKey:(NSString * _Nonnull)key;
		[Export ("removePropertyForKey:")]
		void RemovePropertyForKey (string key);

		// -(void)setPropertyWithDouble:(double)double_ forKey:(NSString * _Nonnull)key;
		[Export ("setPropertyWithDouble:forKey:")]
		void SetPropertyWithDouble (double double_, string key);

		// -(void)setPropertyWithBool:(BOOL)bool_ forKey:(NSString * _Nonnull)key;
		[Export ("setPropertyWithBool:forKey:")]
		void SetPropertyWithBool (bool bool_, string key);

		// -(BOOL)setPropertyWithValue:(id _Nonnull)value forKey:(NSString * _Nonnull)key error:(NSError * _Nullable * _Nullable)error;
		[Export ("setPropertyWithValue:forKey:error:")]
		bool SetPropertyWithValue (NSObject value, string key, [NullAllowed] out NSError error);

		// -(BOOL)setProperties:(id _Nonnull)object error:(NSError * _Nullable * _Nullable)error;
		[Export ("setProperties:error:")]
		bool SetProperties (NSObject @object, [NullAllowed] out NSError error);
	}





	// @interface UACustomEventAccountProperties : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC30UACustomEventAccountProperties")]
	[DisableDefaultCtor]
	interface UACustomEventAccountProperties
	{
		// @property (copy, nonatomic) NSString * _Nullable userID;
		[NullAllowed, Export ("userID")]
		string UserID { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable category;
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; set; }

		// @property (nonatomic) BOOL isLTV;
		[Export ("isLTV")]
		bool IsLTV { get; set; }

		// -(instancetype _Nonnull)initWithCategory:(NSString * _Nullable)category type:(NSString * _Nullable)type isLTV:(BOOL)isLTV userID:(NSString * _Nullable)userID __attribute__((objc_designated_initializer));
		[Export ("initWithCategory:type:isLTV:userID:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string category, [NullAllowed] string type, bool isLTV, [NullAllowed] string userID);
	}

	// @interface UACustomEventAccountTemplate : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC28UACustomEventAccountTemplate")]
	[DisableDefaultCtor]
	interface UACustomEventAccountTemplate
	{
		// +(UACustomEventAccountTemplate * _Nonnull)registered __attribute__((warn_unused_result("")));
		[Static]
		[Export ("registered")]
		UACustomEventAccountTemplate Registered { get; }

		// +(UACustomEventAccountTemplate * _Nonnull)loggedIn __attribute__((warn_unused_result("")));
		[Static]
		[Export ("loggedIn")]
		UACustomEventAccountTemplate LoggedIn { get; }

		// +(UACustomEventAccountTemplate * _Nonnull)loggedOut __attribute__((warn_unused_result("")));
		[Static]
		[Export ("loggedOut")]
		UACustomEventAccountTemplate LoggedOut { get; }
	}

	// @interface UACustomEventMediaProperties : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC28UACustomEventMediaProperties")]
	[DisableDefaultCtor]
	interface UACustomEventMediaProperties
	{
		// @property (copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable category;
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable eventDescription;
		[NullAllowed, Export ("eventDescription")]
		string EventDescription { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable author;
		[NullAllowed, Export ("author")]
		string Author { get; set; }

		// @property (copy, nonatomic) NSDate * _Nullable publishedDate;
		[NullAllowed, Export ("publishedDate", ArgumentSemantic.Copy)]
		NSDate PublishedDate { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable isFeature;
		[NullAllowed, Export ("isFeature", ArgumentSemantic.Strong)]
		NSNumber IsFeature { get; set; }

		// @property (nonatomic) BOOL isLTV;
		[Export ("isLTV")]
		bool IsLTV { get; set; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nullable)id category:(NSString * _Nullable)category type:(NSString * _Nullable)type eventDescription:(NSString * _Nullable)eventDescription isLTV:(BOOL)isLTV author:(NSString * _Nullable)author publishedDate:(NSDate * _Nullable)publishedDate isFeature:(NSNumber * _Nullable)isFeature __attribute__((objc_designated_initializer));
		[Export ("initWithId:category:type:eventDescription:isLTV:author:publishedDate:isFeature:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string id, [NullAllowed] string category, [NullAllowed] string type, [NullAllowed] string eventDescription, bool isLTV, [NullAllowed] string author, [NullAllowed] NSDate publishedDate, [NullAllowed] NSNumber isFeature);
	}

	// @interface UACustomEventMediaTemplate : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC26UACustomEventMediaTemplate")]
	[DisableDefaultCtor]
	interface UACustomEventMediaTemplate
	{
		// +(UACustomEventMediaTemplate * _Nonnull)browsed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("browsed")]
		UACustomEventMediaTemplate Browsed { get; }

		// +(UACustomEventMediaTemplate * _Nonnull)consumed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("consumed")]
		UACustomEventMediaTemplate Consumed { get; }

		// +(UACustomEventMediaTemplate * _Nonnull)sharedWithSource:(NSString * _Nullable)source medium:(NSString * _Nullable)medium __attribute__((warn_unused_result("")));
		[Static]
		[Export ("sharedWithSource:medium:")]
		UACustomEventMediaTemplate SharedWithSource ([NullAllowed] string source, [NullAllowed] string medium);

		// +(UACustomEventMediaTemplate * _Nonnull)starred __attribute__((warn_unused_result("")));
		[Static]
		[Export ("starred")]
		UACustomEventMediaTemplate Starred { get; }
	}

	// @interface UACustomEventRetailProperties : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC29UACustomEventRetailProperties")]
	[DisableDefaultCtor]
	interface UACustomEventRetailProperties
	{
		// @property (copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable category;
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable eventDescription;
		[NullAllowed, Export ("eventDescription")]
		string EventDescription { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable isNewItem;
		[NullAllowed, Export ("isNewItem", ArgumentSemantic.Strong)]
		NSNumber IsNewItem { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable currency;
		[NullAllowed, Export ("currency")]
		string Currency { get; set; }

		// @property (nonatomic) BOOL isLTV;
		[Export ("isLTV")]
		bool IsLTV { get; set; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nullable)id category:(NSString * _Nullable)category type:(NSString * _Nullable)type eventDescription:(NSString * _Nullable)eventDescription isLTV:(BOOL)isLTV brand:(NSString * _Nullable)brand isNewItem:(NSNumber * _Nullable)isNewItem currency:(NSString * _Nullable)currency __attribute__((objc_designated_initializer));
		[Export ("initWithId:category:type:eventDescription:isLTV:brand:isNewItem:currency:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string id, [NullAllowed] string category, [NullAllowed] string type, [NullAllowed] string eventDescription, bool isLTV, [NullAllowed] string brand, [NullAllowed] NSNumber isNewItem, [NullAllowed] string currency);
	}

	// @interface UACustomEventRetailTemplate : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC27UACustomEventRetailTemplate")]
	[DisableDefaultCtor]
	interface UACustomEventRetailTemplate
	{
		// +(UACustomEventRetailTemplate * _Nonnull)browsed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("browsed")]
		UACustomEventRetailTemplate Browsed { get; }

		// +(UACustomEventRetailTemplate * _Nonnull)addedToCart __attribute__((warn_unused_result("")));
		[Static]
		[Export ("addedToCart")]
		UACustomEventRetailTemplate AddedToCart { get; }

		// +(UACustomEventRetailTemplate * _Nonnull)sharedWithSource:(NSString * _Nullable)source medium:(NSString * _Nullable)medium __attribute__((warn_unused_result("")));
		[Static]
		[Export ("sharedWithSource:medium:")]
		UACustomEventRetailTemplate SharedWithSource ([NullAllowed] string source, [NullAllowed] string medium);

		// +(UACustomEventRetailTemplate * _Nonnull)starred __attribute__((warn_unused_result("")));
		[Static]
		[Export ("starred")]
		UACustomEventRetailTemplate Starred { get; }

		// +(UACustomEventRetailTemplate * _Nonnull)purchased __attribute__((warn_unused_result("")));
		[Static]
		[Export ("purchased")]
		UACustomEventRetailTemplate Purchased { get; }

		// +(UACustomEventRetailTemplate * _Nonnull)wishlistWithIdentifier:(NSString * _Nullable)identifier name:(NSString * _Nullable)name __attribute__((warn_unused_result("")));
		[Static]
		[Export ("wishlistWithIdentifier:name:")]
		UACustomEventRetailTemplate WishlistWithIdentifier ([NullAllowed] string identifier, [NullAllowed] string name);
	}

	// @interface UACustomEventSearchProperties : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC29UACustomEventSearchProperties")]
	[DisableDefaultCtor]
	interface UACustomEventSearchProperties
	{
		// @property (copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable query;
		[NullAllowed, Export ("query")]
		string Query { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable totalResults;
		[NullAllowed, Export ("totalResults", ArgumentSemantic.Strong)]
		NSNumber TotalResults { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable category;
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; set; }

		// @property (nonatomic) BOOL isLTV;
		[Export ("isLTV")]
		bool IsLTV { get; set; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nullable)id query:(NSString * _Nullable)query totalResults:(NSNumber * _Nullable)totalResults category:(NSString * _Nullable)category type:(NSString * _Nullable)type isLTV:(BOOL)isLTV __attribute__((objc_designated_initializer));
		[Export ("initWithId:query:totalResults:category:type:isLTV:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string id, [NullAllowed] string query, [NullAllowed] NSNumber totalResults, [NullAllowed] string category, [NullAllowed] string type, bool isLTV);
	}

	// @interface UACustomEventSearchTemplate : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC27UACustomEventSearchTemplate")]
	[DisableDefaultCtor]
	interface UACustomEventSearchTemplate
	{
		// +(UACustomEventSearchTemplate * _Nonnull)search __attribute__((warn_unused_result("")));
		[Static]
		[Export ("search")]
		UACustomEventSearchTemplate Search { get; }
	}

	// @protocol UADeepLinkDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC18UADeepLinkDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UADeepLinkDelegate
	{
		// @required -(void)receivedDeepLink:(NSUrl * _Nonnull)deepLink completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Abstract]
		[Export ("receivedDeepLink:completionHandler:")]
		void CompletionHandler (NSUrl deepLink, Action completionHandler);
	}

	// @interface UAFeature : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC9UAFeature")]
	[DisableDefaultCtor]
	interface UAFeature
	{
		// +(UAFeature * _Nonnull)inAppAutomation __attribute__((warn_unused_result("")));
		[Static]
		[Export ("inAppAutomation")]
		UAFeature InAppAutomation { get; }

		// +(UAFeature * _Nonnull)messageCenter __attribute__((warn_unused_result("")));
		[Static]
		[Export ("messageCenter")]
		UAFeature MessageCenter { get; }

		// +(UAFeature * _Nonnull)push __attribute__((warn_unused_result("")));
		[Static]
		[Export ("push")]
		UAFeature Push { get; }

		// +(UAFeature * _Nonnull)analytics __attribute__((warn_unused_result("")));
		[Static]
		[Export ("analytics")]
		UAFeature Analytics { get; }

		// +(UAFeature * _Nonnull)tagsAndAttributes __attribute__((warn_unused_result("")));
		[Static]
		[Export ("tagsAndAttributes")]
		UAFeature TagsAndAttributes { get; }

		// +(UAFeature * _Nonnull)contacts __attribute__((warn_unused_result("")));
		[Static]
		[Export ("contacts")]
		UAFeature Contacts { get; }

		// +(UAFeature * _Nonnull)featureFlags __attribute__((warn_unused_result("")));
		[Static]
		[Export ("featureFlags")]
		UAFeature FeatureFlags { get; }

		// +(UAFeature * _Nonnull)all __attribute__((warn_unused_result("")));
		[Static]
		[Export ("all")]
		UAFeature All { get; }

		// +(UAFeature * _Nonnull)none __attribute__((warn_unused_result("")));
		[Static]
		[Export ("none")]
		UAFeature None { get; }

		// -(instancetype _Nonnull)initFrom:(NSArray<UAFeature *> * _Nonnull)from;
		[Export ("initFrom:")]
		NativeHandle Constructor (UAFeature[] from);

		// -(BOOL)contains:(UAFeature * _Nonnull)feature __attribute__((warn_unused_result("")));
		[Export ("contains:")]
		bool Contains (UAFeature feature);

		// @property (readonly, nonatomic) NSUInteger hash;
		[Export ("hash")]
		nuint Hash { get; }

		// -(BOOL)isEqual:(id _Nullable)object __attribute__((warn_unused_result("")));
		[Override]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject @object);
	}

	// @interface UAInAppAutomation : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC17UAInAppAutomation")]
	[DisableDefaultCtor]
	interface UAInAppAutomation
	{
		// @property (readonly, nonatomic, strong) UAInAppMessaging * _Nonnull inAppMessaging;
		[Export ("inAppMessaging", ArgumentSemantic.Strong)]
		UAInAppMessaging InAppMessaging { get; }

		// @property (nonatomic) BOOL isPaused;
		[Export ("isPaused")]
		bool IsPaused { get; set; }

		// @property (nonatomic) NSTimeInterval displayInterval;
		[Export ("displayInterval")]
		double DisplayInterval { get; set; }
	}

	// @interface UAInAppMessaging : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC16UAInAppMessaging")]
	interface UAInAppMessaging
	{
	}

	// @interface UAMessageCenter : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC15UAMessageCenter")]
	[DisableDefaultCtor]
	interface UAMessageCenter
	{
		[Wrap ("WeakDisplayDelegate")]
		[NullAllowed]
		UAMessageCenterDisplayDelegate DisplayDelegate { get; set; }

		// @property (nonatomic, weak) id<UAMessageCenterDisplayDelegate> _Nullable displayDelegate;
		[NullAllowed, Export ("displayDelegate", ArgumentSemantic.Weak)]
		NSObject WeakDisplayDelegate { get; set; }

		// @property (readonly, nonatomic, strong) UAMessageCenterInbox * _Nonnull inbox;
		[Export ("inbox", ArgumentSemantic.Strong)]
		UAMessageCenterInbox Inbox { get; }

		// -(BOOL)setThemeFromPlist:(NSString * _Nonnull)plist error:(NSError * _Nullable * _Nullable)error;
		[Export ("setThemeFromPlist:error:")]
		bool SetThemeFromPlist (string plist, [NullAllowed] out NSError error);

		// @property (nonatomic, strong) id<UAMessageCenterPredicate> _Nullable predicate;
		[NullAllowed, Export ("predicate", ArgumentSemantic.Strong)]
		UAMessageCenterPredicate Predicate { get; set; }

		// -(void)display;
		[Export ("display")]
		void Display ();

		// -(void)displayWithMessageID:(NSString * _Nonnull)messageID;
		[Export ("displayWithMessageID:")]
		void DisplayWithMessageID (string messageID);

		// -(void)dismiss;
		[Export ("dismiss")]
		void Dismiss ();
	}

	// @protocol UAMessageCenterDisplayDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC30UAMessageCenterDisplayDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UAMessageCenterDisplayDelegate
	{
		// @required -(void)displayMessageCenterForMessageID:(NSString * _Nonnull)messageID;
		[Abstract]
		[Export ("displayMessageCenterForMessageID:")]
		void DisplayMessageCenterForMessageID (string messageID);

		// @required -(void)displayMessageCenter;
		[Abstract]
		[Export ("displayMessageCenter")]
		void DisplayMessageCenter ();

		// @required -(void)dismissMessageCenter;
		[Abstract]
		[Export ("dismissMessageCenter")]
		void DismissMessageCenter ();
	}

	// @interface UAMessageCenterInbox : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC20UAMessageCenterInbox")]
	interface UAMessageCenterInbox
	{
		// -(void)getMessagesWithCompletionHandler:(void (^ _Nonnull)(NSArray<UAMessageCenterMessage *> * _Nonnull))completionHandler;
		[Export ("getMessagesWithCompletionHandler:")]
		void GetMessagesWithCompletionHandler (Action<UAMessageCenterMessage[]> completionHandler);

		// -(void)getUserWithCompletionHandler:(void (^ _Nonnull)(UAMessageCenterUser * _Nullable))completionHandler;
		[Export ("getUserWithCompletionHandler:")]
		void GetUserWithCompletionHandler (Action<UAMessageCenterUser> completionHandler);

		// -(void)getUnreadCountWithCompletionHandler:(void (^ _Nonnull)(NSInteger))completionHandler;
		[Export ("getUnreadCountWithCompletionHandler:")]
		void GetUnreadCountWithCompletionHandler (Action<nint> completionHandler);

		// -(void)messageForBodyURL:(NSUrl * _Nonnull)bodyURL completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
		[Export ("messageForBodyURL:completionHandler:")]
		void MessageForBodyURL (NSUrl bodyURL, Action<UAMessageCenterMessage> completionHandler);

		// -(void)messageForID:(NSString * _Nonnull)messageID completionHandler:(void (^ _Nonnull)(UAMessageCenterMessage * _Nullable))completionHandler;
		[Export ("messageForID:completionHandler:")]
		void MessageForID (string messageID, Action<UAMessageCenterMessage> completionHandler);

		// -(void)refreshMessagesWithCompletionHandler:(void (^ _Nonnull)(BOOL))completionHandler;
		[Export ("refreshMessagesWithCompletionHandler:")]
		void RefreshMessagesWithCompletionHandler (Action<bool> completionHandler);

		// -(void)markReadWithMessages:(NSArray<UAMessageCenterMessage *> * _Nonnull)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Export ("markReadWithMessages:completionHandler:")]
		void MarkReadWithMessages (UAMessageCenterMessage[] messages, Action completionHandler);

		// -(void)markReadWithMessageIDs:(NSArray<NSString *> * _Nonnull)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Export ("markReadWithMessageIDs:completionHandler:")]
		void MarkReadWithMessageIDs (string[] messageIDs, Action completionHandler);

		// -(void)deleteWithMessages:(NSArray<UAMessageCenterMessage *> * _Nonnull)messages completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Export ("deleteWithMessages:completionHandler:")]
		void DeleteWithMessages (UAMessageCenterMessage[] messages, Action completionHandler);

		// -(void)deleteWithMessageIDs:(NSArray<NSString *> * _Nonnull)messageIDs completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Export ("deleteWithMessageIDs:completionHandler:")]
		void DeleteWithMessageIDs (string[] messageIDs, Action completionHandler);

	}

	// @interface UAMessageCenterMessage : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC22UAMessageCenterMessage")]
	[DisableDefaultCtor]
	interface UAMessageCenterMessage
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull title;
		[Export ("title")]
		string Title { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable subtitle;
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable listIcon;
		[NullAllowed, Export ("listIcon")]
		string ListIcon { get; }

		// @property (readonly, nonatomic) BOOL isExpired;
		[Export ("isExpired")]
		bool IsExpired { get; }

		// @property (readonly, copy, nonatomic) NSDictionary<NSString *,NSString *> * _Nonnull extra;
		[Export ("extra", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> Extra { get; }

		// @property (readonly, copy, nonatomic) NSUrl * _Nonnull bodyURL;
		[Export ("bodyURL", ArgumentSemantic.Copy)]
		NSUrl BodyURL { get; }

		// @property (readonly, copy, nonatomic) NSDate * _Nullable expirationDate;
		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }

		// @property (readonly, copy, nonatomic) NSDate * _Nonnull sentDate;
		[Export ("sentDate", ArgumentSemantic.Copy)]
		NSDate SentDate { get; }

		// @property (readonly, nonatomic) BOOL unread;
		[Export ("unread")]
		bool Unread { get; }

		// +(NSString * _Nullable)parseMessageIDFromUserInfo:(NSDictionary * _Nonnull)userInfo __attribute__((warn_unused_result("")));
		[Static]
		[Export ("parseMessageIDFromUserInfo:")]
		[return: NullAllowed]
		string ParseMessageIDFromUserInfo (NSDictionary userInfo);
	}

	// @protocol UAMessageCenterPredicate
	[Protocol (Name = "_TtP17AirshipObjectiveC24UAMessageCenterPredicate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UAMessageCenterPredicate
	{
		// @required -(BOOL)evaluateWithMessage:(UAMessageCenterMessage * _Nonnull)message __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("evaluateWithMessage:")]
		bool EvaluateWithMessage (UAMessageCenterMessage message);
	}

	// @interface UAMessageCenterTheme : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC20UAMessageCenterTheme")]
	interface UAMessageCenterTheme
	{
		// @property (nonatomic, strong) UIColor * _Nullable refreshTintColor;
		[NullAllowed, Export ("refreshTintColor", ArgumentSemantic.Strong)]
		UIColor RefreshTintColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable refreshTintColorDark;
		[NullAllowed, Export ("refreshTintColorDark", ArgumentSemantic.Strong)]
		UIColor RefreshTintColorDark { get; set; }

		// @property (nonatomic) BOOL iconsEnabled;
		[Export ("iconsEnabled")]
		bool IconsEnabled { get; set; }

		// @property (nonatomic, strong) UIImage * _Nullable placeholderIcon;
		[NullAllowed, Export ("placeholderIcon", ArgumentSemantic.Strong)]
		UIImage PlaceholderIcon { get; set; }

		// @property (nonatomic, strong) UIFont * _Nullable cellTitleFont;
		[NullAllowed, Export ("cellTitleFont", ArgumentSemantic.Strong)]
		UIFont CellTitleFont { get; set; }

		// @property (nonatomic, strong) UIFont * _Nullable cellDateFont;
		[NullAllowed, Export ("cellDateFont", ArgumentSemantic.Strong)]
		UIFont CellDateFont { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellColor;
		[NullAllowed, Export ("cellColor", ArgumentSemantic.Strong)]
		UIColor CellColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellColorDark;
		[NullAllowed, Export ("cellColorDark", ArgumentSemantic.Strong)]
		UIColor CellColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellTitleColor;
		[NullAllowed, Export ("cellTitleColor", ArgumentSemantic.Strong)]
		UIColor CellTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellTitleColorDark;
		[NullAllowed, Export ("cellTitleColorDark", ArgumentSemantic.Strong)]
		UIColor CellTitleColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellDateColor;
		[NullAllowed, Export ("cellDateColor", ArgumentSemantic.Strong)]
		UIColor CellDateColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellDateColorDark;
		[NullAllowed, Export ("cellDateColorDark", ArgumentSemantic.Strong)]
		UIColor CellDateColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellTintColor;
		[NullAllowed, Export ("cellTintColor", ArgumentSemantic.Strong)]
		UIColor CellTintColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cellTintColorDark;
		[NullAllowed, Export ("cellTintColorDark", ArgumentSemantic.Strong)]
		UIColor CellTintColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable unreadIndicatorColor;
		[NullAllowed, Export ("unreadIndicatorColor", ArgumentSemantic.Strong)]
		UIColor UnreadIndicatorColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable unreadIndicatorColorDark;
		[NullAllowed, Export ("unreadIndicatorColorDark", ArgumentSemantic.Strong)]
		UIColor UnreadIndicatorColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable selectAllButtonTitleColor;
		[NullAllowed, Export ("selectAllButtonTitleColor", ArgumentSemantic.Strong)]
		UIColor SelectAllButtonTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable selectAllButtonTitleColorDark;
		[NullAllowed, Export ("selectAllButtonTitleColorDark", ArgumentSemantic.Strong)]
		UIColor SelectAllButtonTitleColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable deleteButtonTitleColor;
		[NullAllowed, Export ("deleteButtonTitleColor", ArgumentSemantic.Strong)]
		UIColor DeleteButtonTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable deleteButtonTitleColorDark;
		[NullAllowed, Export ("deleteButtonTitleColorDark", ArgumentSemantic.Strong)]
		UIColor DeleteButtonTitleColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable markAsReadButtonTitleColor;
		[NullAllowed, Export ("markAsReadButtonTitleColor", ArgumentSemantic.Strong)]
		UIColor MarkAsReadButtonTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable markAsReadButtonTitleColorDark;
		[NullAllowed, Export ("markAsReadButtonTitleColorDark", ArgumentSemantic.Strong)]
		UIColor MarkAsReadButtonTitleColorDark { get; set; }

		// @property (nonatomic) BOOL hideDeleteButton;
		[Export ("hideDeleteButton")]
		bool HideDeleteButton { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable editButtonTitleColor;
		[NullAllowed, Export ("editButtonTitleColor", ArgumentSemantic.Strong)]
		UIColor EditButtonTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable editButtonTitleColorDark;
		[NullAllowed, Export ("editButtonTitleColorDark", ArgumentSemantic.Strong)]
		UIColor EditButtonTitleColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cancelButtonTitleColor;
		[NullAllowed, Export ("cancelButtonTitleColor", ArgumentSemantic.Strong)]
		UIColor CancelButtonTitleColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable cancelButtonTitleColorDark;
		[NullAllowed, Export ("cancelButtonTitleColorDark", ArgumentSemantic.Strong)]
		UIColor CancelButtonTitleColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable backButtonColor;
		[NullAllowed, Export ("backButtonColor", ArgumentSemantic.Strong)]
		UIColor BackButtonColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable backButtonColorDark;
		[NullAllowed, Export ("backButtonColorDark", ArgumentSemantic.Strong)]
		UIColor BackButtonColorDark { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable navigationBarTitle;
		[NullAllowed, Export ("navigationBarTitle")]
		string NavigationBarTitle { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageListBackgroundColor;
		[NullAllowed, Export ("messageListBackgroundColor", ArgumentSemantic.Strong)]
		UIColor MessageListBackgroundColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageListBackgroundColorDark;
		[NullAllowed, Export ("messageListBackgroundColorDark", ArgumentSemantic.Strong)]
		UIColor MessageListBackgroundColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageListContainerBackgroundColor;
		[NullAllowed, Export ("messageListContainerBackgroundColor", ArgumentSemantic.Strong)]
		UIColor MessageListContainerBackgroundColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageListContainerBackgroundColorDark;
		[NullAllowed, Export ("messageListContainerBackgroundColorDark", ArgumentSemantic.Strong)]
		UIColor MessageListContainerBackgroundColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageViewBackgroundColor;
		[NullAllowed, Export ("messageViewBackgroundColor", ArgumentSemantic.Strong)]
		UIColor MessageViewBackgroundColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageViewBackgroundColorDark;
		[NullAllowed, Export ("messageViewBackgroundColorDark", ArgumentSemantic.Strong)]
		UIColor MessageViewBackgroundColorDark { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageViewContainerBackgroundColor;
		[NullAllowed, Export ("messageViewContainerBackgroundColor", ArgumentSemantic.Strong)]
		UIColor MessageViewContainerBackgroundColor { get; set; }

		// @property (nonatomic, strong) UIColor * _Nullable messageViewContainerBackgroundColorDark;
		[NullAllowed, Export ("messageViewContainerBackgroundColorDark", ArgumentSemantic.Strong)]
		UIColor MessageViewContainerBackgroundColorDark { get; set; }
	}

	// @interface UAMessageCenterUser : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC19UAMessageCenterUser")]
	[DisableDefaultCtor]
	interface UAMessageCenterUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull password;
		[Export ("password")]
		string Password { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull username;
		[Export ("username")]
		string Username { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull basicAuthString;
		[Export ("basicAuthString")]
		string BasicAuthString { get; }
	}

	// @interface UAMessageCenterViewControllerFactory : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC36UAMessageCenterViewControllerFactory")]
	interface UAMessageCenterViewControllerFactory
	{
		// +(UIViewController * _Nonnull)makeWithTheme:(UAMessageCenterTheme * _Nullable)theme predicate:(id<UAMessageCenterPredicate> _Nullable)predicate __attribute__((warn_unused_result("")));
		[Static]
		[Export ("makeWithTheme:predicate:")]
		UIViewController MakeWithTheme ([NullAllowed] UAMessageCenterTheme theme, [NullAllowed] UAMessageCenterPredicate predicate);

		// +(UIViewController * _Nullable)makeWithThemePlist:(NSString * _Nullable)themePlist error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("makeWithThemePlist:error:")]
		[return: NullAllowed]
		UIViewController MakeWithThemePlist ([NullAllowed] string themePlist, [NullAllowed] out NSError error);

		// +(UIViewController * _Nullable)makeWithThemePlist:(NSString * _Nullable)themePlist predicate:(id<UAMessageCenterPredicate> _Nullable)predicate error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("makeWithThemePlist:predicate:error:")]
		[return: NullAllowed]
		UIViewController MakeWithThemePlist ([NullAllowed] string themePlist, [NullAllowed] UAMessageCenterPredicate predicate, [NullAllowed] out NSError error);

		// +(UIView * _Nonnull)embedWithTheme:(UAMessageCenterTheme * _Nullable)theme predicate:(id<UAMessageCenterPredicate> _Nullable)predicate in:(UIViewController * _Nonnull)parentViewController __attribute__((warn_unused_result("")));
		[Static]
		[Export ("embedWithTheme:predicate:in:")]
		UIView EmbedWithTheme ([NullAllowed] UAMessageCenterTheme theme, [NullAllowed] UAMessageCenterPredicate predicate, UIViewController parentViewController);
	}

	// @interface UANotificationCategories : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC24UANotificationCategories")]
	interface UANotificationCategories
	{
		// +(NSSet<UNNotificationCategory *> * _Nonnull)defaultCategories __attribute__((warn_unused_result("")));
		[Static]
		[Export ("defaultCategories")]
		NSSet<UNNotificationCategory> DefaultCategories { get; }
	}

	// @interface UAPreferenceCenter : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC18UAPreferenceCenter")]
	[DisableDefaultCtor]
	interface UAPreferenceCenter
	{
		[Wrap ("WeakOpenDelegate")]
		[NullAllowed]
		UAPreferenceCenterOpenDelegate OpenDelegate { get; set; }

		// @property (nonatomic, weak) id<UAPreferenceCenterOpenDelegate> _Nullable openDelegate;
		[NullAllowed, Export ("openDelegate", ArgumentSemantic.Weak)]
		NSObject WeakOpenDelegate { get; set; }

		// -(BOOL)setThemeFromPlist:(NSString * _Nonnull)plist error:(NSError * _Nullable * _Nullable)error;
		[Export ("setThemeFromPlist:error:")]
		bool SetThemeFromPlist (string plist, [NullAllowed] out NSError error);

		// -(void)openPreferenceCenter:(NSString * _Nonnull)preferenceCenterID;
		[Export ("openPreferenceCenter:")]
		void OpenPreferenceCenter (string preferenceCenterID);

		// -(void)jsonConfigWithPreferenceCenterID:(NSString * _Nonnull)preferenceCenterID completionHandler:(void (^ _Nonnull)(NSData * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("jsonConfigWithPreferenceCenterID:completionHandler:")]
		void JsonConfigWithPreferenceCenterID (string preferenceCenterID, Action<NSData, NSError> completionHandler);
	}

	// @protocol UAPreferenceCenterOpenDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC30UAPreferenceCenterOpenDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UAPreferenceCenterOpenDelegate
	{
		// @required -(BOOL)openPreferenceCenter:(NSString * _Nonnull)preferenceCenterID __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("openPreferenceCenter:")]
		bool OpenPreferenceCenter (string preferenceCenterID);
	}

	// @interface UAPreferenceCenterViewControllerFactory : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC39UAPreferenceCenterViewControllerFactory")]
	interface UAPreferenceCenterViewControllerFactory
	{
		// +(UIViewController * _Nonnull)makeViewControllerWithPreferenceCenterID:(NSString * _Nonnull)preferenceCenterID __attribute__((warn_unused_result("")));
		[Static]
		[Export ("makeViewControllerWithPreferenceCenterID:")]
		UIViewController MakeViewControllerWithPreferenceCenterID (string preferenceCenterID);

		// +(UIViewController * _Nullable)makeViewControllerWithPreferenceCenterID:(NSString * _Nonnull)preferenceCenterID preferenceCenterThemePlist:(NSString * _Nonnull)preferenceCenterThemePlist error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("makeViewControllerWithPreferenceCenterID:preferenceCenterThemePlist:error:")]
		[return: NullAllowed]
		UIViewController MakeViewControllerWithPreferenceCenterID (string preferenceCenterID, string preferenceCenterThemePlist, [NullAllowed] out NSError error);

		// +(UIView * _Nullable)embedWithPreferenceCenterID:(NSString * _Nonnull)preferenceCenterID preferenceCenterThemePlist:(NSString * _Nullable)preferenceCenterThemePlist in:(UIViewController * _Nonnull)parentViewController error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Static]
		[Export ("embedWithPreferenceCenterID:preferenceCenterThemePlist:in:error:")]
		[return: NullAllowed]
		UIView EmbedWithPreferenceCenterID (string preferenceCenterID, [NullAllowed] string preferenceCenterThemePlist, UIViewController parentViewController, [NullAllowed] out NSError error);
	}

	// @interface UAPrivacyManager : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC16UAPrivacyManager")]
	[DisableDefaultCtor]
	interface UAPrivacyManager
	{
		// @property (nonatomic, strong) UAFeature * _Nonnull enabledFeatures;
		[Export ("enabledFeatures", ArgumentSemantic.Strong)]
		UAFeature EnabledFeatures { get; set; }

		// -(void)enableFeatures:(UAFeature * _Nonnull)features;
		[Export ("enableFeatures:")]
		void EnableFeatures (UAFeature features);

		// -(void)disableFeatures:(UAFeature * _Nonnull)features;
		[Export ("disableFeatures:")]
		void DisableFeatures (UAFeature features);

		// -(BOOL)isEnabled:(UAFeature * _Nonnull)features __attribute__((warn_unused_result("")));
		[Export ("isEnabled:")]
		bool IsEnabled (UAFeature features);

		// -(BOOL)isAnyFeatureEnabled __attribute__((warn_unused_result("")));
		[Export ("isAnyFeatureEnabled")]
		bool IsAnyFeatureEnabled { get; }
	}

	// @interface UAPush : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC6UAPush")]
	[DisableDefaultCtor]
	interface UAPush
	{
		// @property (nonatomic) BOOL backgroundPushNotificationsEnabled;
		[Export ("backgroundPushNotificationsEnabled")]
		bool BackgroundPushNotificationsEnabled { get; set; }

		// @property (nonatomic) BOOL userPushNotificationsEnabled;
		[Export ("userPushNotificationsEnabled")]
		bool UserPushNotificationsEnabled { get; set; }

		// @property (nonatomic) BOOL requestExplicitPermissionWhenEphemeral;
		[Export ("requestExplicitPermissionWhenEphemeral")]
		bool RequestExplicitPermissionWhenEphemeral { get; set; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable deviceToken;
		[NullAllowed, Export ("deviceToken")]
		string DeviceToken { get; }

		// @property (nonatomic) UNAuthorizationOptions notificationOptions;
		[Export ("notificationOptions", ArgumentSemantic.Assign)]
		UNAuthorizationOptions NotificationOptions { get; set; }

		// @property (copy, nonatomic) NSSet<UNNotificationCategory *> * _Nonnull customCategories;
		[Export ("customCategories", ArgumentSemantic.Copy)]
		NSSet<UNNotificationCategory> CustomCategories { get; set; }

		// @property (readonly, copy, nonatomic) NSSet<UNNotificationCategory *> * _Nonnull combinedCategories;
		[Export ("combinedCategories", ArgumentSemantic.Copy)]
		NSSet<UNNotificationCategory> CombinedCategories { get; }

		// @property (nonatomic) BOOL requireAuthorizationForDefaultCategories;
		[Export ("requireAuthorizationForDefaultCategories")]
		bool RequireAuthorizationForDefaultCategories { get; set; }

		[Wrap ("WeakPushNotificationDelegate")]
		[NullAllowed]
		UAPushNotificationDelegate PushNotificationDelegate { get; set; }

		// @property (nonatomic, weak) id<UAPushNotificationDelegate> _Nullable pushNotificationDelegate;
		[NullAllowed, Export ("pushNotificationDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPushNotificationDelegate { get; set; }

		[Wrap ("WeakRegistrationDelegate")]
		[NullAllowed]
		UARegistrationDelegate RegistrationDelegate { get; set; }

		// @property (nonatomic, weak) id<UARegistrationDelegate> _Nullable registrationDelegate;
		[NullAllowed, Export ("registrationDelegate", ArgumentSemantic.Weak)]
		NSObject WeakRegistrationDelegate { get; set; }

		// @property (readonly, nonatomic, strong) UNNotificationResponse * _Nullable launchNotificationResponse;
		[NullAllowed, Export ("launchNotificationResponse", ArgumentSemantic.Strong)]
		UNNotificationResponse LaunchNotificationResponse { get; }

		// @property (readonly, nonatomic, strong) UAAuthorizedNotificationSettings * _Nonnull authorizedNotificationSettings;
		[Export ("authorizedNotificationSettings", ArgumentSemantic.Strong)]
		UAAuthorizedNotificationSettings AuthorizedNotificationSettings { get; }

		// @property (readonly, nonatomic) UNAuthorizationStatus authorizationStatus;
		[Export ("authorizationStatus")]
		UNAuthorizationStatus AuthorizationStatus { get; }

		// @property (readonly, nonatomic) BOOL userPromptedForNotifications;
		[Export ("userPromptedForNotifications")]
		bool UserPromptedForNotifications { get; }

		// @property (nonatomic) UNNotificationPresentationOptions defaultPresentationOptions;
		[Export ("defaultPresentationOptions", ArgumentSemantic.Assign)]
		UNNotificationPresentationOptions DefaultPresentationOptions { get; set; }

		// -(void)enableUserPushNotificationsWithCompletionHandler:(void (^ _Nonnull)(BOOL))completionHandler;
		[Export ("enableUserPushNotificationsWithCompletionHandler:")]
		void EnableUserPushNotificationsWithCompletionHandler (Action<bool> completionHandler);

		// @property (readonly, nonatomic) BOOL isPushNotificationsOptedIn;
		[Export ("isPushNotificationsOptedIn")]
		bool IsPushNotificationsOptedIn { get; }

		// -(void)setBadgeNumber:(NSInteger)badge completionHandler:(void (^ _Nonnull)(NSError * _Nullable))completionHandler;
		[Export ("setBadgeNumber:completionHandler:")]
		void SetBadgeNumber (nint badge, Action<NSError> completionHandler);

		// @property (readonly, nonatomic) NSInteger badgeNumber;
		[Export ("badgeNumber")]
		nint BadgeNumber { get; }

		// @property (nonatomic) BOOL autobadgeEnabled;
		[Export ("autobadgeEnabled")]
		bool AutobadgeEnabled { get; set; }

		// -(void)resetBadgeWithCompletionHandler:(void (^ _Nonnull)(NSError * _Nullable))completionHandler;
		[Export ("resetBadgeWithCompletionHandler:")]
		void ResetBadgeWithCompletionHandler (Action<NSError> completionHandler);


		// @property (nonatomic, strong) NSTimeZone * _Nullable timeZone;
		[NullAllowed, Export ("timeZone", ArgumentSemantic.Strong)]
		NSTimeZone TimeZone { get; set; }

		// @property (nonatomic) BOOL quietTimeEnabled;
		[Export ("quietTimeEnabled")]
		bool QuietTimeEnabled { get; set; }

		// -(void)setQuietTimeStartHour:(NSInteger)startHour startMinute:(NSInteger)startMinute endHour:(NSInteger)endHour endMinute:(NSInteger)endMinute;
		[Export ("setQuietTimeStartHour:startMinute:endHour:endMinute:")]
		void SetQuietTimeStartHour (nint startHour, nint startMinute, nint endHour, nint endMinute);
	}

	// @protocol UAPushNotificationDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC26UAPushNotificationDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UAPushNotificationDelegate
	{
		// @required -(void)receivedForegroundNotification:(NSDictionary * _Nonnull)userInfo completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Abstract]
		[Export ("receivedForegroundNotification:completionHandler:")]
		void ReceivedForegroundNotification (NSDictionary userInfo, Action completionHandler);

		// @required -(void)receivedBackgroundNotification:(NSDictionary * _Nonnull)userInfo completionHandler:(void (^ _Nonnull)(UIBackgroundFetchResult))completionHandler;
		[Abstract]
		[Export ("receivedBackgroundNotification:completionHandler:")]
		void ReceivedBackgroundNotification (NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler);

		// @required -(void)receivedNotificationResponse:(UNNotificationResponse * _Nonnull)notificationResponse completionHandler:(void (^ _Nonnull)(void))completionHandler;
		[Abstract]
		[Export ("receivedNotificationResponse:completionHandler:")]
		void ReceivedNotificationResponse (UNNotificationResponse notificationResponse, Action completionHandler);

		// @required -(void)extendPresentationOptions:(UNNotificationPresentationOptions)options notification:(UNNotification * _Nonnull)notification completionHandler:(void (^ _Nonnull)(UNNotificationPresentationOptions))completionHandler;
		[Abstract]
		[Export ("extendPresentationOptions:notification:completionHandler:")]
		void ExtendPresentationOptions (UNNotificationPresentationOptions options, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler);
	}

	// @protocol UARegistrationDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC22UARegistrationDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UARegistrationDelegate
	{
		// @required -(void)notificationRegistrationFinishedWithAuthorizedSettings:(UAAuthorizedNotificationSettings * _Nonnull)authorizedSettings categories:(NSSet<UNNotificationCategory *> * _Nonnull)categories status:(UNAuthorizationStatus)status;
		[Abstract]
		[Export ("notificationRegistrationFinishedWithAuthorizedSettings:categories:status:")]
		void NotificationRegistrationFinishedWithAuthorizedSettings (UAAuthorizedNotificationSettings authorizedSettings, NSSet<UNNotificationCategory> categories, UNAuthorizationStatus status);

		// @required -(void)notificationRegistrationFinishedWithAuthorizedSettings:(UAAuthorizedNotificationSettings * _Nonnull)authorizedSettings status:(UNAuthorizationStatus)status;
		[Abstract]
		[Export ("notificationRegistrationFinishedWithAuthorizedSettings:status:")]
		void NotificationRegistrationFinishedWithAuthorizedSettings (UAAuthorizedNotificationSettings authorizedSettings, UNAuthorizationStatus status);

		// @required -(void)notificationAuthorizedSettingsDidChange:(UAAuthorizedNotificationSettings * _Nonnull)authorizedSettings;
		[Abstract]
		[Export ("notificationAuthorizedSettingsDidChange:")]
		void NotificationAuthorizedSettingsDidChange (UAAuthorizedNotificationSettings authorizedSettings);

		// @required -(void)apnsRegistrationSucceededWithDeviceToken:(NSData * _Nonnull)deviceToken;
		[Abstract]
		[Export ("apnsRegistrationSucceededWithDeviceToken:")]
		void ApnsRegistrationSucceededWithDeviceToken (NSData deviceToken);

		// @required -(void)apnsRegistrationFailedWithError:(NSError * _Nonnull)error;
		[Abstract]
		[Export ("apnsRegistrationFailedWithError:")]
		void ApnsRegistrationFailedWithError (NSError error);
	}

	// @interface UAScopedSubscriptionListEditor : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC30UAScopedSubscriptionListEditor")]
	interface UAScopedSubscriptionListEditor
	{
		// -(void)subscribe:(NSString * _Nonnull)subscriptionListID scope:(enum UAChannelScope)scope;
		[Export ("subscribe:scope:")]
		void Subscribe (string subscriptionListID, UAChannelScope scope);

		// -(void)unsubscribe:(NSString * _Nonnull)subscriptionListID scope:(enum UAChannelScope)scope;
		[Export ("unsubscribe:scope:")]
		void Unsubscribe (string subscriptionListID, UAChannelScope scope);

		// -(void)apply;
		[Export ("apply")]
		void Apply ();
	}

	// @interface UASubscriptionListEditor : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC24UASubscriptionListEditor")]
	interface UASubscriptionListEditor
	{
		// -(void)subscribe:(NSString * _Nonnull)subscriptionListID;
		[Export ("subscribe:")]
		void Subscribe (string subscriptionListID);

		// -(void)unsubscribe:(NSString * _Nonnull)subscriptionListID;
		[Export ("unsubscribe:")]
		void Unsubscribe (string subscriptionListID);

		// -(void)apply;
		[Export ("apply")]
		void Apply ();
	}

	// @interface UATagEditor : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC11UATagEditor")]
	interface UATagEditor
	{
		// -(void)addTags:(NSArray<NSString *> * _Nonnull)tags;
		[Export ("addTags:")]
		void AddTags (string[] tags);

		// -(void)addTag:(NSString * _Nonnull)tag;
		[Export ("addTag:")]
		void AddTag (string tag);

		// -(void)removeTags:(NSArray<NSString *> * _Nonnull)tags;
		[Export ("removeTags:")]
		void RemoveTags (string[] tags);

		// -(void)removeTag:(NSString * _Nonnull)tag;
		[Export ("removeTag:")]
		void RemoveTag (string tag);

		// -(void)setTags:(NSArray<NSString *> * _Nonnull)tags;
		[Export ("setTags:")]
		void SetTags (string[] tags);

		// -(void)clearTags;
		[Export ("clearTags")]
		void ClearTags ();

		// -(void)apply;
		[Export ("apply")]
		void Apply ();
	}

	// @interface UATagGroupsEditor : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC17UATagGroupsEditor")]
	interface UATagGroupsEditor
	{
		// -(void)addTags:(NSArray<NSString *> * _Nonnull)tags group:(NSString * _Nonnull)group;
		[Export ("addTags:group:")]
		void AddTags (string[] tags, string group);

		// -(void)removeTags:(NSArray<NSString *> * _Nonnull)tags group:(NSString * _Nonnull)group;
		[Export ("removeTags:group:")]
		void RemoveTags (string[] tags, string group);

		// -(void)setTags:(NSArray<NSString *> * _Nonnull)tags group:(NSString * _Nonnull)group;
		[Export ("setTags:group:")]
		void SetTags (string[] tags, string group);

		// -(void)apply;
		[Export ("apply")]
		void Apply ();
	}

	// @enum UAPermission
	public enum UAPermission : long
	{
		DisplayNotifications = 0,
		Location = 1
	}

	// @enum UAPermissionStatus
	public enum UAPermissionStatus : long
	{
		NotDetermined = 0,
		Granted = 1,
		Denied = 2
	}

	// @protocol UAAirshipPermissionDelegate
	[Protocol (Name = "_TtP17AirshipObjectiveC27UAAirshipPermissionDelegate_"), Model]
	[BaseType (typeof(NSObject))]
	interface UAAirshipPermissionDelegate
	{
		// @required -(void)checkPermissionStatusWithCompletionHandler:(void (^ _Nonnull)(enum UAPermissionStatus))completionHandler;
		[Abstract]
		[Export ("checkPermissionStatusWithCompletionHandler:")]
		void CheckPermissionStatus (Action<UAPermissionStatus> completionHandler);

		// @required -(void)requestPermissionWithCompletionHandler:(void (^ _Nonnull)(enum UAPermissionStatus))completionHandler;
		[Abstract]
		[Export ("requestPermissionWithCompletionHandler:")]
		void RequestPermission (Action<UAPermissionStatus> completionHandler);
	}

	// @interface UAPermissionsManager : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC20UAPermissionsManager")]
	[DisableDefaultCtor]
	interface UAPermissionsManager
	{
		// -(void)setDelegate:(id<UAAirshipPermissionDelegate> _Nullable)delegate permission:(enum UAPermission)permission;
		[Export ("setDelegate:permission:")]
		void SetDelegate ([NullAllowed] UAAirshipPermissionDelegate @delegate, UAPermission permission);

		// -(void)checkPermissionStatus:(enum UAPermission)permission completionHandler:(void (^ _Nonnull)(enum UAPermissionStatus))completionHandler;
		[Export ("checkPermissionStatus:completionHandler:")]
		void CheckPermissionStatus (UAPermission permission, Action<UAPermissionStatus> completionHandler);

		// -(void)requestPermission:(enum UAPermission)permission completionHandler:(void (^ _Nonnull)(enum UAPermissionStatus))completionHandler;
		[Export ("requestPermission:completionHandler:")]
		void RequestPermission (UAPermission permission, Action<UAPermissionStatus> completionHandler);

		// -(void)requestPermission:(enum UAPermission)permission enableAirshipUsageOnGrant:(BOOL)enableAirshipUsageOnGrant completionHandler:(void (^ _Nonnull)(enum UAPermissionStatus))completionHandler;
		[Export ("requestPermission:enableAirshipUsageOnGrant:completionHandler:")]
		void RequestPermission (UAPermission permission, bool enableAirshipUsageOnGrant, Action<UAPermissionStatus> completionHandler);
	}

	// @interface UAirship : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC17AirshipObjectiveC8UAirship")]
	interface UAirship
	{
		// @property (readonly, nonatomic, strong, class) UAPush * _Nonnull push;
		[Static]
		[Export ("push", ArgumentSemantic.Strong)]
		UAPush Push { get; }

		// @property (readonly, nonatomic, strong, class) UAChannel * _Nonnull channel;
		[Static]
		[Export ("channel", ArgumentSemantic.Strong)]
		UAChannel Channel { get; }

		// @property (readonly, nonatomic, strong, class) UAContact * _Nonnull contact;
		[Static]
		[Export ("contact", ArgumentSemantic.Strong)]
		UAContact Contact { get; }

		// @property (readonly, nonatomic, strong, class) UAAnalytics * _Nonnull analytics;
		[Static]
		[Export ("analytics", ArgumentSemantic.Strong)]
		UAAnalytics Analytics { get; }

		// @property (readonly, nonatomic, strong, class) UAMessageCenter * _Nonnull messageCenter;
		[Static]
		[Export ("messageCenter", ArgumentSemantic.Strong)]
		UAMessageCenter MessageCenter { get; }

		// @property (readonly, nonatomic, strong, class) UAPreferenceCenter * _Nonnull preferenceCenter;
		[Static]
		[Export ("preferenceCenter", ArgumentSemantic.Strong)]
		UAPreferenceCenter PreferenceCenter { get; }

		// @property (readonly, nonatomic, strong, class) UAPrivacyManager * _Nonnull privacyManager;
		[Static]
		[Export ("privacyManager", ArgumentSemantic.Strong)]
		UAPrivacyManager PrivacyManager { get; }

		// @property (readonly, nonatomic, strong, class) UAInAppAutomation * _Nonnull inAppAutomation;
		[Static]
		[Export ("inAppAutomation", ArgumentSemantic.Strong)]
		UAInAppAutomation InAppAutomation { get; }

		// @property (readonly, nonatomic, strong, class) UAPermissionsManager * _Nonnull permissionsManager;
		[Static]
		[Export ("permissionsManager", ArgumentSemantic.Strong)]
		UAPermissionsManager PermissionsManager { get; }

		[Wrap ("WeakDeepLinkDelegate"), Static]
		[NullAllowed]
		UADeepLinkDelegate DeepLinkDelegate { get; set; }

		// @property (nonatomic, strong, class) id<UADeepLinkDelegate> _Nullable deepLinkDelegate;
		[Static]
		[NullAllowed, Export ("deepLinkDelegate", ArgumentSemantic.Strong)]
		NSObject WeakDeepLinkDelegate { get; set; }

		// +(BOOL)takeOffWithLaunchOptions:(NSDictionary<UIApplicationLaunchOptionsKey,id> * _Nullable)launchOptions error:(NSError * _Nullable * _Nullable)error;
		[Static]
		[Export ("takeOffWithLaunchOptions:error:")]
		bool TakeOffWithLaunchOptions ([NullAllowed] NSDictionary<NSString, NSObject> launchOptions, [NullAllowed] out NSError error);

		// +(BOOL)takeOff:(UAConfig * _Nullable)config launchOptions:(NSDictionary<UIApplicationLaunchOptionsKey,id> * _Nullable)launchOptions error:(NSError * _Nullable * _Nullable)error;
		[Static]
		[Export ("takeOff:launchOptions:error:")]
		bool TakeOff ([NullAllowed] UAConfig config, [NullAllowed] NSDictionary<NSString, NSObject> launchOptions, [NullAllowed] out NSError error);
	}
}
