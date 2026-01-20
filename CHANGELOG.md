# Airship DotNet Changelog

## Version 21.0.0 - January 19, 2026
Major release that moves Message Center inbox functionality into the main package and updates native SDKs to 20.1.1.

### Changes
- Updated iOS SDK to 20.1.1
- Updated Android SDK to 20.1.1
- Moved Message Center inbox functionality (`IAirshipMessageCenter`, `Message` model) from `Airship.Net.MessageCenter` to `Airship.Net`
- Changed Message Center access pattern from extension method to static property: `Airship.Instance.MessageCenter()` → `Airship.MessageCenter`
- `Airship.Net.MessageCenter` package now contains only MAUI UI components (Controls)
- Updated iOS minimum version to iOS 16+
- Updated MAUI Controls dependency to 9.0.0

See the [Migration Guide](MIGRATION.md) for upgrade instructions.

## Version 20.2.1 - December 30, 2025
Minor release to update SDKs and resolve crashes caused by calling iOS SDK methods on background threads instead of the main thread.

### Changes
- Resolve crashes caused by calling iOS SDK methods on background threads instead of the main thread.
- Updated iOS SDK to 19.11.5
- Updated Android SDK to 19.13.6

## Version 20.2.0 - October 7, 2025
Minor release that updates the SDKs and restores onMessageCenterDisplay.

### Changes
- Restored onMessageCenterDisplay that was erroneously removed in version 20.0.0.
- Updated iOS SDK to 19.11.0
- Updated Android SDK to 19.13.4

## Version 20.1.0 - August 25, 2025
Minor release that updates the SDKs and fixes a build signing issue with iOS.

### Changes
- Removed embedded framework that failed to be signed during a release build.
- Updated iOS SDK to 19.8.2
- Updated Android SDK to 19.11.0


## Version 20.0.0 - August 1, 2025
Major release with complete interface modernization and architectural improvements.

### Changes
- Updated iOS SDK to 19.6.1
- Updated Android SDK to 19.8.0
- Complete interface modernization - split monolithic IAirship into module-specific interfaces
- Changed access pattern from instance-based to static module properties
- All async operations now return Tasks
- Merged Message Center functionality into main Airship.Net package
- Added iOS AirshipWrapper to handle Swift async method compatibility
- Improved type safety and separation of concerns across modules

## Version 19.5.0 - Feb 7, 2025
Minor release that updates the Android SDK to 18.7.0, including AndroidX library updates.

### Changes
- Updated Android SDK to 18.7.0

## Version 19.4.1 - Dec 12, 2024
Minor release that updates the Airship.Net package to no longer depend on MAUI and adds methods to fetch channel and contact subscription lists to the cross-platform library.

### Changes
- Removed unnecessary MAUI dependency from Airship.Net
- Added `FetchChannelSubscriptionLists` and `FetchContactSubscriptionLists` methods to Airship.Net

## Version 19.4.0 - July 29, 2024
Minor release that updates the Airship SDK to iOS 17.10.1 and Android 17.8.1.

### Changes
- Updated iOS SDK to 17.10.1
- Updated Android SDK to 17.8.1

## Version 19.3.0 - Apr 8, 2024
Minor release that updates the Airship SDK to iOS 17.10.0 and Android 17.7.4.

### Changes
- Updated iOS SDK to 17.10.0
- Updated Android SDK to 17.7.4
- iOS: Fixed issue with frequency checks being checked before the message is ready to display
- Android: Fixed channel ID creation delay after enabling a feature when none was enabled
- Android: Fixed a potential NPE when reading from intent extras on API 33

## Version 19.2.0 - Mar 15, 2024
Minor release that updates the Airship SDK to iOS 17.9.0 and Android 17.7.3, and expands plist theming options available for Message Center.

### Changes
- Updated iOS SDK to 17.9.0
- Updated Android SDK to 17.7.3
- Support for dark mode and extended theming options for Message Center plist
- Fixed an iOS bug in the Message Center message delete and mark read methods in Airship.Net

## Version 19.1.0 - Jan 25, 2024
Minor release that updates the Airship SDK to iOS 17.7.3 and Android 17.7.2, fixes an iOS custom event properties reporting issue, and Android contact subscription list editing. Apps that target iOS and make use of custom events or Android and make use of contact subscription editing should update.

### Changes
- Updated iOS SDK to 17.7.3
- Updated Android SDK to 17.7.2
- Fixed a bug that prevented custom event properties from being reported on iOS
- Fixed contact subscription list updates (`EditContactSubscriptionLists`) on Android
- Deprecated iOS `Trace` log level and add the replacement `Verbose` log level.

## Version 19.0.0 - Nov 21, 2023
Major release that updates the Airship bindings and cross-platform libraries to target .NET 8.0. The Airship .NET SDK now requires .NET 8.0 (`net8.0-android` and `net8.0-ios`) as the minimum target framework, and iOS 14+ as the minimum deployment version with Xcode 16+.

### Changes
- Updated iOS SDK to 17.6.1
- Resolved build issues in Windows Visual Studio. Linked Mac builds are now working as expected.

## Version 18.0.0 - Nov 10, 2023
Major release that updates to Airship SDK 17.x. This release adds support for Stories, In-App experiences downstream of a sequence in Journeys, and improves SDK auth. The .NET SDK now requires .NET 7.0 (`net7.0-android` and `net7.0-ios`) as the minimum target framework, and iOS 14+ as the minimum deployment version with Xcode 14.3+.

### Changes
- Updated iOS SDK to 17.6.0
- Updated Android SDK to 17.5.0
- Added the ability to update Channel and Contact subscriptions to the common .NET library
- Removed Channel update listener in favor of a new notification status listener.
- Partially fixed issues with building from Windows Visual Studio. Linked Mac builds are not currently working. See [Known Issues](#known-issues) for more details.

See the [Migration Guide](https://github.com/urbanairship/airship-dotnet/tree/main/MIGRATION.md) for further details.

### Known Issues
Build/run via a linked Mac from Visual Studio on Windows is not currently working as expected. This appears to be a known issue and is expected to be fixed in the upcoming .NET 8 release. In our testing, this issue impacts other SDKs that make use of XCFrameworks, and is not limited to Airship SDKs. We will continue monitoring the situation and update with any new workarounds or fixes that become available.

Builds and runs performed directly on a Mac are not impacted by this issue.

Related issue: https://github.com/xamarin/xamarin-macios/issues/19173#issuecomment-1790490792

## Version 17.1.0 - July 24, 2023
Minor release that updates Airship SDKs to the latest 16.x releases and fixes issues with bitcode for iOS.

### Changes
- Android SDK version 16.11.1
- iOS SDK version 16.12.3

## Version 17.0.0 - March 17, 2023
Major release to support MAUI. The Airship .NET SDK targets .NET 6.0, and is compatible with
Android 5.0+ (API 21+) and iOS 13+, using the latest supported release of Xcode (currently 14.2).

### Changes
- Android SDK version 16.9.0
- iOS SDK version 16.11.2
