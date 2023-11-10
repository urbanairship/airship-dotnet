# Airship Xamarin Changelog

## Version 18.0.0 - Nov 10, 2023
Major release that updates to Airship SDK 17.x. This release adds support for Stories, In-App experiences downstream of a sequence in Journeys, and improves SDK auth. The .NET SDK now requires .NET 7.0 (`net7.0-android` and `net7.0-ios`) as the minimum target framework, and iOS 14+ as the minimum deployment version with Xcode 14.3+.

## Changes
- Updated iOS SDK to 17.6.0
- Updated Android SDK to 17.5.0
- Added the ability to update Channel and Contact subscriptions to the common .NET library
- Removed Channel update listener in favor of a new notification status listener.
- Partially fixed issues with building from Windows Visual Studio. Linked Mac builds are not currently working. See [Known Issues](#known-issues) for more details.

See the [Migration Guide](https://github.com/urbanairship/airship-dotnet/tree/main/MIGRATION.md) for further details.

## Known Issues
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
