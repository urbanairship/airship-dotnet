# Airship .NET

The Airship .NET SDK exposes a common subset of functionality between
the iOS and Android SDKs. This library is built against .NET 10.0, and can be used
within shared codebases (e.g., a MAUI app).

## Compatibility

The Airship .NET SDK targets the following platforms:
- `net10.0`
- `net10.0-android`
- `net10.0-ios`

The Airship .NET SDK is compatible with:
- Android 5.0 (API 21) or higher.
- iOS 16 or higher, using the latest supported release of Xcode.

## Resources
- [Getting started guide](https://docs.airship.com/platform/mobile/setup/sdk/maui/)
- [Mobile platform documentation](https://docs.airship.com/platform/mobile/)

## Setup

Use NuGet to install the `airship.net` package.

### Optional Feature Packages

* `airship.net.messagecenter` - Provides a cross-platform control that can be used to display Message Center messages.

Detailed instructions can be found in the [Getting started guide](https://docs.airship.com/platform/mobile/setup/sdk/maui/).

A cross-platform sample app is provided in the `MauiSample` directory on
[Github](https://github.com/urbanairship/airship-dotnet/tree/main/MauiSample).
