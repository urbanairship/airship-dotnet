# Android Development Setup for Airship .NET MAUI

Quick setup guide for Airship .NET MAUI on Android (assumes Android dev environment already set up).

## Setup Steps

### 1. Install .NET 8.0.100
```bash
brew install --cask dotnet-sdk
dotnet --version  # Should show 8.0.100
```

### 2. Install .NET Workloads
```bash
dotnet workload install maui android maui-android
```

### 3. Build and Run
```bash
cd MauiSample
./run-android.sh
```

## Configuration Files

The required config files should already exist:
- `MauiSample/Platforms/Android/Assets/google-services.json` (dummy file)
- `MauiSample/Platforms/Android/Assets/airshipconfig.properties` (dev keys)

## Troubleshooting

**"Build failed"**: Run `./gradlew clean && ./gradlew :binderator:build` from project root

**"Insufficient storage"**: Wipe emulator data in Android Studio AVD Manager

**App won't install**: Try `adb uninstall com.urbanairship.sample` then re-run script

## Quick Commands
```bash
# Build and run
./run-android.sh

# Manual launch existing app
adb shell monkey -p com.urbanairship.sample -c android.intent.category.LAUNCHER 1

# Check emulator storage
adb shell df /data
```