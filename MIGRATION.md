# Migration Guide

## 20.x to 21.x

### .NET Version

This version of the plugin now requires .NET 9.0 (`net9.0-android` and `net9.0-ios`) as the minimum target framework.

### Minimum SDK Versions

- **iOS**: Requires iOS 16+ and Xcode 16+ (Swift 6.0)
- **Android**: Requires Android API 21+ (`SupportedOSPlatformVersion` 21.0)

### Native SDK Updates

The underlying native SDKs have been updated to version 20.1.1:

| Platform | Previous Version | New Version |
|----------|-----------------|-------------|
| iOS | 19.11.x | 20.1.1 |
| Android | 19.13.x | 20.1.1 |

### Message Center API Changes

Message Center inbox functionality has been moved from the `Airship.Net.MessageCenter` package into the main `Airship.Net` package. The access pattern has changed from an extension method to a static property:

```csharp
// 20.x - Extension method
var messages = await Airship.Instance.MessageCenter().GetMessages();
await Airship.Instance.MessageCenter().MarkRead(messageId);
await Airship.Instance.MessageCenter().Display();

// 21.x - Static property
var messages = await Airship.MessageCenter.GetMessages();
await Airship.MessageCenter.MarkRead(messageId);
await Airship.MessageCenter.Display();
```

### Package Changes

The `Airship.Net.MessageCenter` package now contains **only MAUI UI components** (Controls). All core Message Center functionality (inbox operations, message models) is now in the main `Airship.Net` package.

| Package | 20.x Contents | 21.x Contents |
|---------|--------------|---------------|
| `Airship.Net` | Core SDK functionality | Core SDK + Message Center inbox API |
| `Airship.Net.MessageCenter` | Message Center API + MAUI UI | MAUI UI components only |

If you only need Message Center inbox functionality (getting messages, marking read, etc.) without the MAUI UI components, you no longer need to reference `Airship.Net.MessageCenter`.

### Android Module Changes

The Message Center and Preference Center modules have been split into core and UI modules:

| Previous Module | New Modules |
|-----------------|-------------|
| `urbanairship-message-center` | `urbanairship-message-center-core` + `urbanairship-message-center` |
| `urbanairship-preference-center` | `urbanairship-preference-center-core` + `urbanairship-preference-center` |

The `-core` modules contain data models and API functionality, while the original modules now contain only the View-based UI components and depend on `-core`.

### Android API Changes

#### ActionRegistry

The `ActionRegistry.Entry.Predicate` property is now read-only and the `Entry.SetPredicate()` method has been removed. Use `ActionRegistry.UpdateEntry()` to modify predicates:

```csharp
// 20.x
var entry = Airship.ActionRegistry.GetEntry("my_action");
entry.SetPredicate(args => args.Situation == Situation.ManualInvocation);
// or
entry.Predicate = myPredicate;

// 21.x
Airship.ActionRegistry.UpdateEntry("my_action", myPredicate);
```

The `IPredicate` interface has been renamed to `IActionPredicate`. Update any custom predicate implementations:

```csharp
// 20.x
public class MyPredicate : Java.Lang.Object, IPredicate
{
    public bool Apply(ActionArguments args) => true;
}

// 21.x
public class MyPredicate : Java.Lang.Object, IActionPredicate
{
    public bool Apply(ActionArguments args) => true;
}
```

#### PreferenceDataStore

The `OnPreferenceChange` event and underlying `AddListener`/`RemoveListener` methods have been removed from the native SDK:

```csharp
// 20.x
preferenceDataStore.OnPreferenceChange += (key) => { Console.WriteLine($"Changed: {key}"); };

// 21.x - this API is no longer available
// Consider using alternative state management approaches
```

### Dependency Updates

AndroidX and other dependencies have been updated. Key version changes:

| Dependency | Previous | New |
|------------|----------|-----|
| androidx.lifecycle | 2.8.x | 2.9.x |
| androidx.fragment | 1.8.2 | 1.8.9 |
| androidx.core | 1.13.x | 1.17.x |
| androidx.room | 2.6.x | 2.8.x |
| kotlin-stdlib | 2.0.x | 2.2.x |
| kotlinx-coroutines | 1.9.x | 1.10.x |

### Build Configuration

Update your project files to target .NET 9:

```xml
<!-- Before -->
<TargetFrameworks>net8.0-android;net8.0-ios</TargetFrameworks>

<!-- After -->
<TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
```

Update conditional compilation checks:

```xml
<!-- Before -->
<ItemGroup Condition="'$(TargetFramework)' == 'net8.0-android'">

<!-- After -->
<ItemGroup Condition="'$(TargetFramework)' == 'net9.0-android'">
```

## 19.x to 20.x

### Architecture Changes

The monolithic `IAirship` interface has been split into focused, module-specific interfaces:

| Module | Interface | Access Via |
|--------|-----------|------------|
| Push | `IAirshipPush` | `Airship.Push` |
| Channel | `IAirshipChannel` | `Airship.Channel` |
| Contact | `IAirshipContact` | `Airship.Contact` |
| Message Center | `IAirshipMessageCenter` | `Airship.Instance.MessageCenter()` * |
| Analytics | `IAirshipAnalytics` | `Airship.Analytics` |
| In-App | `IAirshipInApp` | `Airship.InApp` |
| Privacy | `IAirshipPrivacyManager` | `Airship.PrivacyManager` |
| Feature Flags | `IAirshipFeatureFlagManager` | `Airship.FeatureFlagManager` |
| Preference Center | `IAirshipPreferenceCenter` | `Airship.PreferenceCenter` |

\* Message Center was accessed via extension method in 20.x. In 21.x, it was changed to `Airship.MessageCenter` for consistency with other modules.

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