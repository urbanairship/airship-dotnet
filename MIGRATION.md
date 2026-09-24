# Migration Guide

## 21.x to 22.x

### Native SDK Updates

| Platform | Previous Version | New Version |
|----------|-----------------|-------------|
| iOS | 20.12.x | 21.0.2 |
| Android | 20.12.x | 21.0.2 |

The .NET package major is intentionally ahead of the native major: **Airship.Net 22.x wraps Airship SDK 21.x**. This has been true since Airship.Net 21.x wrapped Airship SDK 20.x.

### Minimum SDK Versions

- **Android**: now requires **API 26** (Android 8.0), raised from API 21. The native SDK uses `java.time.Instant`, which requires API 26 and is not desugared.
- **iOS**: unchanged at iOS 16+.

```xml
<!-- Before -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>

<!-- After -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">26.0</SupportedOSPlatformVersion>
```

### Event API Changes

Message Center events moved off the Message Center module onto the `Airship.Instance` facade, alongside the other lifecycle events. Delegate signatures are unchanged — only the location and name differ. These are plugin changes, not native SDK changes, so they are not described in the Airship SDK 21 release notes.

| 21.x | 22.x | Signature |
|------|------|-----------|
| `Airship.MessageCenter.OnMessagesUpdated` | `Airship.Instance.OnMessageCenterUpdated` | `EventHandler<EventArgs>` — unchanged |
| `Airship.MessageCenter.OnMessageCenterDisplay` | `Airship.Instance.OnDisplayMessageCenter` | `EventHandler<MessageCenterEventArgs>` — unchanged |
| `Airship.Instance.OnMessageCenterDisplay` | `Airship.Instance.OnDisplayMessageCenter` | `EventHandler<MessageCenterEventArgs>` — unchanged |

```csharp
// 21.x
Airship.MessageCenter.OnMessagesUpdated += OnMessagesUpdated;
Airship.MessageCenter.OnMessageCenterDisplay += OnMessageCenterDisplay;

// 22.x
Airship.Instance.OnMessageCenterUpdated += OnMessagesUpdated;
Airship.Instance.OnDisplayMessageCenter += OnMessageCenterDisplay;
```

Both events were removed from `IAirshipMessageCenter`, so `Airship.MessageCenter.OnMessagesUpdated` no longer resolves.

#### Push event types consolidated

| 21.x | 22.x |
|------|------|
| `AirshipEventType.BackgroundPushReceived` | `AirshipEventType.PushReceived` |
| `AirshipEventType.BackgroundNotificationResponse` | `AirshipEventType.NotificationResponse` |
| `AirshipEventType.ForegroundNotificationResponse` | `AirshipEventType.NotificationResponse` |

### New Permission Values

`Permission` gains six values, for the system-permission actions Scenes can now request. If you `switch` over `Permission` without a default case, add them:

`AppTrackingTransparency`, `Camera`, `Microphone`, `Bluetooth`, `PhotoLibrary`, `Contacts`

### New Privacy Feature: `Features.OnDeviceAI`

`Features.OnDeviceAI` gates the SDK's on-device AI functionality and is included in `Features.All`.

**If you set an explicit feature set rather than `Features.All`, you must add `Features.OnDeviceAI` to opt in.** Otherwise on-device AI stays disabled, with no error. The underlying AI APIs (`Airship.ai` on Android, `AirshipFoundationModels` on iOS) are not yet exposed through this plugin; only the privacy flag is.

```csharp
// Opt in explicitly
Airship.PrivacyManager.EnabledFeatures = Features.Push | Features.Analytics | Features.OnDeviceAI;
```

### Removed iOS APIs

Removed from the native iOS SDK, and therefore no longer bound:

- `UAMessageCenterUser`
- `UAMessageCenterNativeBridge` / `UAMessageCenterNativeBridgeDelegate`
- `UAMessageCenterInbox.GetUserWithCompletionHandler`
- `UAAppIntegration.Application(UIApplication, Action<UIBackgroundFetchResult>)` — the background-fetch forwarding method, deprecated by Apple in iOS 13. Remove the corresponding call from your `AppDelegate`; use background push or `BGAppRefreshTask` instead.

The Message Center user APIs existed to load a message's `bodyURL` in your own web view, authenticating with the user's basic auth string. **That flow is no longer supported**: a message can now be delivered as a native (Scenes) layout whose `bodyURL` does not point to renderable web content, so loading it in a web view silently breaks. Use the `MessageView` control from `Airship.Net.MessageCenter` instead.

### iOS Framework Changes

The Scene/layout rendering engine has been split out of `AirshipCore` into `AirshipSceneRenderer` and `AirshipScenes`. If you integrate the iOS xcframeworks manually instead of using the `Airship.Net.iOS.ObjectiveC` package, you must add and embed both or the app will fail to link. `AirshipFoundationModels.xcframework` also ships in the SDK 21 bundle; it is optional and currently unused by this plugin.

CocoaPods is no longer supported by the Airship iOS SDK.

### New: Custom Views in Scenes

The new `Airship.Net.CustomViews` package lets a Scene render a MAUI view you supply. Register a factory by the name the Scene references:

```csharp
AirshipCustomViewManager.Shared.Register("weather", args =>
{
    // args.PropertiesJson carries the Scene's JSON properties, if any.
    return new WeatherView();
});
```

Register before a Scene using the view can display — typically at app startup. Returning `null` renders an empty view.

### New: On-Device AI Context

SDK 21 adds on-device AI, which can suppress in-app messages, pick embedded content, and infer Scene text input. Your app can supply context for those evaluations through `Airship.AI`:

```csharp
// Applies to any usage without a provider of its own.
Airship.AI.SetDefaultContextProvider(() =>
    AirshipEvaluationContext.FromStrings("Loyalty tier: gold", "Favorite category: hiking"));

// Or for one specific usage. The subject arrives as JSON.
Airship.AI.SetContextProvider(AirshipAIUsage.InAppMessageSuppression, subjectJson =>
    new AirshipEvaluationContext(new[]
    {
        new AirshipEvaluationContextItem("Last booked: 2026-09-01", priority: -1)
    }));
```

Context goes to the model running the evaluation and nowhere else — Airship does not receive, store, or report it. Providers are invoked on the path to displaying the feature, so keep the work light; the callbacks are deliberately synchronous.

A usage-specific provider **replaces** the default for that usage rather than adding to it. Lower `priority` is more important; the highest values are dropped first if the prompt exceeds the model's input window.

This requires `Features.OnDeviceAI` to be enabled. Supplying your own AI model is not exposed through this plugin.

### .NET MAUI 10

`Microsoft.Maui.Controls` has been updated from **9.0.0 to 10.0.110**. The plugin previously
pinned MAUI 9 while targeting `net10.0`; this aligns the two.

Update the reference in your app:

```xml
<!-- Before -->
<PackageReference Include="Microsoft.Maui.Controls" Version="9.0.0" />

<!-- After -->
<PackageReference Include="Microsoft.Maui.Controls" Version="10.0.110" />
```

MAUI 10 is also what makes iOS 27 support possible — see below.

### iOS 27: UIScene Lifecycle Is Required

**This affects your app only if you build it with Xcode 27** (that is, against the iOS 27 SDK).
Apps built against earlier SDKs are unaffected, and this is independent of Airship.

iOS 27 terminates apps at launch that were built against the iOS 27 SDK but have not adopted the
UIScene lifecycle. The crash is an `EXC_BREAKPOINT` in
`__UIApplicationEvaluateRuntimeIssueForNoSceneLifecycleAdoption`, before any of your code runs.

Add the following to your iOS `Info.plist`:

```xml
<key>UIApplicationSceneManifest</key>
<dict>
    <key>UIApplicationSupportsMultipleScenes</key>
    <false/>
    <key>UISceneConfigurations</key>
    <dict>
        <key>UIWindowSceneSessionRoleApplication</key>
        <array>
            <dict>
                <key>UISceneConfigurationName</key>
                <string>__MAUI_DEFAULT_SCENE_CONFIGURATION__</string>
                <key>UISceneDelegateClassName</key>
                <string>Microsoft_Maui_MauiUISceneDelegate</string>
            </dict>
        </array>
    </dict>
</dict>
```

**Both string values must be exact.** Getting either wrong produces a silent black screen at
launch rather than an error, which is difficult to diagnose:

- `__MAUI_DEFAULT_SCENE_CONFIGURATION__` is the value of
  `MauiUIApplicationDelegate.MauiSceneConfigurationKey`. `MauiUISceneDelegate.WillConnect`
  compares against it and skips window creation if it does not match.
- `Microsoft_Maui_MauiUISceneDelegate` is the Objective-C name .NET for iOS exports for
  `Microsoft.Maui.MauiUISceneDelegate`. The managed name `MauiUISceneDelegate` will not
  resolve; UIKit logs `could not load class with name ...` and no window is created.

This requires MAUI 10. MAUI 9 does not ship `MauiUISceneDelegate`, so there is no way to adopt
the scene lifecycle on it.

### Build Requirements

Building **this plugin from source** now requires **Xcode 27** and **JDK 17**. Apps consuming the published NuGet packages are unaffected and do not need Xcode 27.

Note that the GA .NET iOS workload (26.5.x) asserts an exact Xcode 26.6 match and fails with error `E0191` under Xcode 27. The assertion only runs for projects that produce an app bundle, so the shipped libraries build cleanly; `MauiSample` sets `<ValidateXcodeVersion>false</ValidateXcodeVersion>` to skip it. If your own app project hits `E0191`, the same property is the workaround.

### Verify Your Scenes

SDK 21 includes a broad set of fixes to the Scene layout engine on both platforms. No code changes are required, but **verify your live Scenes render as intended after upgrading**, especially any tuned to look right under SDK 20. On Android, `AirshipEmbeddedInfo.Priority` now reports its real value rather than always `0`, so embedded content sorted by priority may reorder.

### What Did *Not* Change

Several breaking changes in the native SDK 21 migration guides are invisible through the .NET surface. You do **not** need to act on:

- The Android `Long`/`Date` → `java.time.Instant` and `Long` → `kotlin.time.Duration` type migration — handled inside the plugin.
- The Android `EventType` / `EventAutomationTriggerType` rename from `IN_APP_*` to `LAYOUT_*` — not exposed. `AirshipEventType` in this plugin is unrelated.
- The Android `@RestrictTo` tightening and `PushManagerExtensions` removal.
- The iOS `import AirshipScenes` requirement for embedded and custom views — handled inside the plugin.

## 20.x to 21.x

### .NET Version

This version of the plugin now requires .NET 10.0 (`net10.0-android` and `net10.0-ios`) as the minimum target framework.

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

Update your project files to target .NET 10:

```xml
<!-- Before -->
<TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>

<!-- After -->
<TargetFrameworks>net10.0-android;net10.0-ios</TargetFrameworks>
```

Update conditional compilation checks:

```xml
<!-- Before -->
<ItemGroup Condition="'$(TargetFramework)' == 'net9.0-android'">

<!-- After -->
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
```

## 19.x to 20.x

### Architecture Changes

The monolithic `IAirship` interface has been split into focused, module-specific interfaces:

| Module | Interface | Access Via |
|--------|-----------|------------|
| Push | `IAirshipPush` | `Airship.Push` |
| Channel | `IAirshipChannel` | `Airship.Channel` |
| Contact | `IAirshipContact` | `Airship.Contact` |
| Message Center | `IAirshipMessageCenter` | `Airship.Instance.MessageCenter()` |
| Analytics | `IAirshipAnalytics` | `Airship.Analytics` |
| In-App | `IAirshipInApp` | `Airship.InApp` |
| Privacy | `IAirshipPrivacyManager` | `Airship.PrivacyManager` |
| Feature Flags | `IAirshipFeatureFlagManager` | `Airship.FeatureFlagManager` |
| Preference Center | `IAirshipPreferenceCenter` | `Airship.PreferenceCenter` |

**Note:** Message Center access changed in 21.x from `Airship.Instance.MessageCenter()` to `Airship.MessageCenter`. See the [20.x to 21.x](#20x-to-21x) section for details.

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