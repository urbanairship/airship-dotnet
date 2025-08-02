# Migration Guide

## 19.x to 20.x

### Architecture Changes

The monolithic `IAirship` interface has been split into focused, module-specific interfaces:

| Module | Interface | Access Via |
|--------|-----------|------------|
| Push | `IAirshipPush` | `Airship.Push` |
| Channel | `IAirshipChannel` | `Airship.Channel` |
| Contact | `IAirshipContact` | `Airship.Contact` |
| Message Center | `IAirshipMessageCenter` | `Airship.MessageCenter` |
| Analytics | `IAirshipAnalytics` | `Airship.Analytics` |
| In-App | `IAirshipInApp` | `Airship.InApp` |
| Privacy | `IAirshipPrivacyManager` | `Airship.PrivacyManager` |
| Feature Flags | `IAirshipFeatureFlagManager` | `Airship.FeatureFlagManager` |
| Preference Center | `IAirshipPreferenceCenter` | `Airship.PreferenceCenter` |

### API Changes

#### Access Pattern

| 19.x | 20.x |
|------|------|
| `Airship.Instance.UserNotificationsEnabled = true;` | `Airship.Push.UserNotificationsEnabled = true;` |
| `Airship.Instance.ChannelId` | `Airship.Channel.ChannelId` |
| `Airship.Instance.Tags` | `Airship.Channel.Tags` |
| `Airship.Instance.EnabledFeatures` | `Airship.PrivacyManager.EnabledFeatures` |

#### Async Methods

All methods that perform I/O operations now return Tasks:

| 19.x | 20.x |
|------|------|
| `Airship.Instance.GetNamedUser(namedUser => { ... });` | `var namedUser = await Airship.Contact.GetNamedUserID();` |
| `Airship.Instance.InboxMessages(messages => { ... });` | `var messages = await Airship.MessageCenter.GetMessages();` |
| `Airship.Instance.MessageCenterUnreadCount(count => { ... });` | `var count = await Airship.MessageCenter.GetUnreadCount();` |
| `Airship.Instance.FetchChannelSubscriptionLists(lists => { ... });` | `var lists = await Airship.Channel.FetchSubscriptionLists();` |

#### iOS Specific

iOS builds now require the AirshipWrapper framework to handle Swift async method compatibility issues. This is included automatically when building the iOS bindings.

## 18.x to 19.x

### .NET Version

This version of the plugin now requires .NET 8.0 (`net8.0-android` and `net8.0-ios`) as the min target framework.

### Minimum iOS Version

This version of the plugin requires iOS 14+ as the min deployment target and Xcode 16+.

### iOS Log Levels

The `TRACE` level has been renamed to `VERBOSE`, for consistency with other platforms/frameworks.

## 17.x to 18.x

### .NET Version

This version of the plugin now requires .NET 7.0 (`net7.0-android` and `net7.0-ios`) as the min target framework.

### Minimum iOS Version

This version of the plugin now requires iOS 14+ as the min deployment target and Xcode 14.3+.

### API Changes

#### Methods

| 17.x | 18.x |
|------|------|
| `Airship.Instance.NamedUser = "some named user ID";` | `Airship.Instance.IdentifyContact("some named user ID");` |
| `Airship.Instance.NamedUser = null;` | `Airship.Instance.ResetContact();` |
| `var namedUser = Airship.Instance.NamedUser;` | `Airship.Instance.GetNamedUser(namedUser => { ... });` |
| `Airship.Instance.EditNamedUserTagGroups();` | `Airship.Instance.EditContactTagGroups();` |
| `Airship.Instance.EditNamedUserAttributes();` | `Airship.Instance.EditContactAttributes();` |
| `var messages = Airship.Instance.InboxMessages;` | `Airship.Instance.InboxMessages(messages => { ... });` |
| `var count = Airship.Instance.MessageCenterUnreadCount;` | `Airship.Instance.MessageCenterUnreadCount(count => { ... });` |
| `var count = Airship.Instance.MessageCenterCount;` | `Airship.Instance.MessageCenterCount(count => { ... });` |

### API Additions

#### Push notification status Listener

```csharp
Airship.Instance.OnPushNotificationStatusUpdate -= OnPushNotificationStatusEvent;

private void OnPushNotificationStatusEvent(object sender, PushNotificationStatusEventArgs e) => 
{
	bool isUserNotificationsEnabled = e.IsUserNotificationsEnabled;
	// ...
};
```

#### Editing Channel Subscription Lists

```csharp
Airship.Instance.EditChannelSubscriptionLists()
    .subscribe("food");
    .unsubscribe("sports");
    .apply();
```

#### Editing Contact Subscription Lists

```csharp
Airship.Instance.EditContactSubscriptionLists()
    .subscribe("food", "app")
    .unsubscribe("sports", "sms")
    .apply()
```

### API Removals

#### `Airship.Instance.OnChannelUpdate`

Replace with either `OnChannelCreation` or `OnPushNotificationStatusUpdate`, depending on usage.