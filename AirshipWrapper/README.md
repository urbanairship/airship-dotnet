# AirshipWrapper

A minimal Objective-C wrapper around Airship iOS SDK to address Swift async method marshaling issues in Xamarin.iOS.

## Purpose

This wrapper provides a hybrid approach:
- Wraps only the 4 problematic async methods that cause marshaling crashes
- Returns real SDK objects (UAChannel, UAContact, etc.) for everything else
- Acts as a pass-through layer to prevent Swift async marshaling issues

## Prerequisites

The parent directory must have Airship SDK frameworks built with Carthage:
```bash
# In parent directory:
carthage update --use-xcframeworks --no-use-binaries --platform iOS
```

## Building

The AirshipWrapper uses the Airship frameworks from the parent directory's Carthage build.

To build the wrapper:
```bash
xcodebuild -project AirshipWrapper.xcodeproj -scheme AirshipWrapper -configuration Release
```

Or to create an xcframework:
```bash
xcodebuild -create-xcframework \
  -framework build/Release-iphoneos/AirshipWrapper.framework \
  -framework build/Release-iphonesimulator/AirshipWrapper.framework \
  -output AirshipWrapper.xcframework
```

## Project Structure

- `AWAirshipWrapper.h/.m` - Main wrapper class with 4 async method wrappers
- `AirshipWrapper.xcodeproj` - Xcode project configured to use parent Carthage frameworks
- No local Carthage setup needed - uses parent directory's build

## Integration

The built `AirshipWrapper.xcframework` embeds all SDK frameworks and should be referenced in the Xamarin binding project.