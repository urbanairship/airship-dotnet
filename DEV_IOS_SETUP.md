# iOS Development Setup for Airship .NET MAUI

Quick setup guide for Airship .NET MAUI on iOS (assumes Xcode and iOS dev environment already set up).

## Setup Steps

### 1. Install .NET 8.0.100
```bash
brew install --cask dotnet-sdk
dotnet --version  # Should show 8.0.100
```

### 2. Install .NET Workloads
```bash
dotnet workload install maui ios maui-ios
```

### 3. Install Carthage
```bash
brew install carthage
```

### 4. Create iOS Provisioning Profile
Create provisioning profile named: `dotnet-maui-sample-profile` for bundle ID: `com.urbanairship.richpush`

### 5. Build and Run
```bash
cd MauiSample
./run-ios.sh        # Normal build and run
./run-ios.sh --clean # Clean Carthage and rebuild wrapper
```

The script automatically:
- Builds Carthage dependencies if missing
- Builds AirshipWrapper.xcframework if missing  
- Copies wrapper to binding project

## Configuration Files

The required config file should already exist:
- `MauiSample/Platforms/iOS/AirshipConfig.plist` (sample config with dev keys)

## Troubleshooting

**"No provisioning profiles"**: Create profile in Xcode with exact name `dotnet-maui-sample-profile`

**"Build failed"**: Run `./gradlew clean && ./gradlew :binderator:build` from project root

**"Carthage failed"**: Try `brew upgrade carthage`

## Quick Commands
```bash
# Build and run
./run-ios.sh

# Clean build (rebuilds Carthage + wrapper)
./run-ios.sh --clean

# List available simulators
xcrun simctl list devices available

# Reset simulator
xcrun simctl erase all
```