#!/bin/bash

# Build and run Airship .NET MAUI iOS sample app
# Works for any user on macOS
#
# Usage:
#   ./run-ios.sh        - Normal build and run
#   ./run-ios.sh --clean - Clean Carthage and rebuild wrapper before running
#   ./run-ios.sh --fast  - Skip Carthage rebuild, only rebuild wrapper if source changed

set -e  # Exit on any error

# Parse command line arguments
CLEAN_BUILD=false
FAST_BUILD=false
if [[ "$1" == "--clean" ]]; then
    CLEAN_BUILD=true
    echo "🧹 Clean build requested - will rebuild Carthage and wrapper"
elif [[ "$1" == "--fast" ]]; then
    FAST_BUILD=true
    echo "🚀 Fast build requested - will skip Carthage rebuild and only rebuild wrapper if needed"
fi

# Get the script directory to find the project
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Ensure .NET is in PATH (prefer arm64 build if available)
if [[ -f "$HOME/.dotnet10arm64/dotnet" ]]; then
    export PATH="$HOME/.dotnet10arm64:$PATH"
else
    export PATH="$HOME/.dotnet:$PATH"
fi

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET is not installed or not in PATH"
    echo "Please install .NET 8 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Check if we're in the right directory
if [[ ! -f "$PROJECT_DIR/MauiSample.csproj" ]]; then
    echo "❌ Error: MauiSample.csproj not found in $PROJECT_DIR"
    echo "Please run this script from the MauiSample directory"
    exit 1
fi

# Check UseProjectReferences setting
USE_PROJECT_REFS=$(grep -o '<UseProjectReferences>.*</UseProjectReferences>' MauiSample.csproj | sed 's/<[^>]*>//g')

if [[ "$USE_PROJECT_REFS" = "true" ]]; then
    echo "✅ Using project references (development mode)"
    echo "   SDK changes will be built automatically with the sample app"
else
    echo "⚠️  Using NuGet packages"
    echo "   To use project references for development, set UseProjectReferences=true in MauiSample.csproj"
fi

# Function to build Carthage dependencies
build_carthage() {
    echo "🏗️  Building Carthage dependencies..."
    cd "$REPO_ROOT"
    
    if [[ "$CLEAN_BUILD" == "true" ]]; then
        rm -rf Carthage/
    fi
    
    if [[ "$FAST_BUILD" == "true" ]] && [[ -d "Carthage/Build" ]]; then
        echo "🚀 Fast mode: Skipping Carthage rebuild (using existing build)"
    elif [[ ! -d "Carthage/Build" ]]; then
        echo "📦 Installing Carthage dependencies..."
        # Try binaries first (faster), fall back to building from source if needed
        carthage update --use-xcframeworks --platform iOS || \
        carthage update --use-xcframeworks --no-use-binaries --platform iOS
    else
        echo "✅ Carthage dependencies already built"
    fi
}

# Function to build AirshipWrapper
build_wrapper() {
    echo "🏗️  Building AirshipWrapper..."
    cd "$REPO_ROOT/AirshipWrapper"
    
    WRAPPER_NEEDS_BUILD=false
    
    if [[ "$CLEAN_BUILD" == "true" ]] || [[ ! -f "lib/AirshipWrapper.xcframework/Info.plist" ]]; then
        WRAPPER_NEEDS_BUILD=true
    else
        # Check if source files are newer than the built framework
        if [[ "AirshipWrapper/AWAirshipWrapper.m" -nt "lib/AirshipWrapper.xcframework/Info.plist" ]] || \
           [[ "AirshipWrapper/AWAirshipWrapper.h" -nt "lib/AirshipWrapper.xcframework/Info.plist" ]]; then
            echo "📝 Source files changed, rebuilding wrapper..."
            WRAPPER_NEEDS_BUILD=true
        fi
    fi
    
    if [[ "$WRAPPER_NEEDS_BUILD" == "true" ]]; then
        echo "🔨 Building AirshipWrapper.xcframework..."
        ./build-wrapper.sh
        
        # Copy to binding project
        echo "📁 Copying wrapper to binding project..."
        mkdir -p "$REPO_ROOT/src/AirshipBindings.iOS.ObjectiveC/lib"
        cp -R lib/AirshipWrapper.xcframework "$REPO_ROOT/src/AirshipBindings.iOS.ObjectiveC/lib/"
    else
        echo "✅ AirshipWrapper already built and up to date"
    fi
}

# Check and build dependencies if needed
if [[ "$USE_PROJECT_REFS" = "true" ]]; then
    # Check for carthage command
    if ! command -v carthage &> /dev/null; then
        echo "❌ Error: Carthage is not installed"
        echo "Please install Carthage: brew install carthage"
        exit 1
    fi
    
    build_carthage
    build_wrapper
fi

# Get booted iPhone/iPad simulators only (excluding Apple TV, Apple Watch, etc.)
BOOTED_IOS_DEVICES=$(xcrun simctl list devices | grep -E "iPhone|iPad" | grep "(Booted)" | awk -F'[()]' '{print $(NF-1)}')

if [[ -z "$BOOTED_IOS_DEVICES" ]]; then
    echo "❌ Error: No iPhone or iPad Simulator is currently running"
    echo "Please start an iPhone or iPad Simulator first"
    # Show available devices
    echo ""
    echo "Available iOS devices:"
    xcrun simctl list devices | grep -E "iPhone|iPad" | grep -v "(Booted)" | head -10
    exit 1
fi

# If multiple iOS devices are booted, use the first one
DEVICE_ID=$(echo "$BOOTED_IOS_DEVICES" | head -n1)
DEVICE_COUNT=$(echo "$BOOTED_IOS_DEVICES" | wc -l | tr -d ' ')

if [[ $DEVICE_COUNT -gt 1 ]]; then
    echo "⚠️  Multiple iOS simulators are running. Using the first one:"
    xcrun simctl list devices | grep "$DEVICE_ID"
fi

echo "🏗️  Building iOS app..."
cd "$PROJECT_DIR"

# Clean previous builds to ensure fresh build
echo "🧹 Cleaning previous builds..."
dotnet clean MauiSample.csproj -f net10.0-ios -v quiet

# Build the app
dotnet build MauiSample.csproj -f net10.0-ios \
    -p:RuntimeIdentifier=iossimulator-arm64 \
    -p:ValidateXcodeVersion=false \
    -p:CodesignEntitlements="" \
    -v minimal

if [[ $? -ne 0 ]]; then
    echo "❌ Build failed. Make sure you have:"
    echo "   1. .NET 8 SDK installed"
    echo "   2. iOS workloads installed (dotnet workload install ios)"
    echo "   3. Provisioning profile 'dotnet-maui-sample-profile' available"
    exit 1
fi

# Uninstall existing app to ensure clean state
echo "🗑️  Uninstalling existing app..."
xcrun simctl uninstall "$DEVICE_ID" com.urbanairship.richpush 2>/dev/null || true

echo "📱 Installing app to simulator (Device: $DEVICE_ID)..."
xcrun simctl install "$DEVICE_ID" "$PROJECT_DIR/bin/Debug/net10.0-ios/iossimulator-arm64/MauiSample.app"

echo "🚀 Launching app..."

# Create log directory with PST timestamp
LOG_BASE_DIR="$SCRIPT_DIR/Sample Run Logs"
mkdir -p "$LOG_BASE_DIR"

# Get PST timestamp (TZ=America/Los_Angeles ensures PST/PDT)
PST_TIMESTAMP=$(TZ=America/Los_Angeles date +"%Y-%m-%d_%H-%M-%S_PST")
LOG_DIR="$LOG_BASE_DIR/$PST_TIMESTAMP"
mkdir -p "$LOG_DIR"

# Create log files
CONSOLE_LOG="$LOG_DIR/console.log"
SYSTEM_LOG="$LOG_DIR/system.log"

echo "📋 Logging to directory: $LOG_DIR"
echo "   Console output: console.log" 
echo "   System logs: system.log"
echo "📋 Press Ctrl+C to stop..."
echo "========================================"
echo ""

# Start system log capture in background (filtered for app-relevant logs)
# Stream to both file and console
xcrun simctl spawn "$DEVICE_ID" log stream \
    --predicate 'processImagePath CONTAINS "MauiSample" OR 
                 messageType == error OR 
                 eventMessage CONTAINS "Airship" OR 
                 eventMessage CONTAINS "xamarin" OR
                 eventMessage CONTAINS "mono" OR
                 eventMessage CONTAINS "SIGABRT" OR
                 eventMessage CONTAINS "crash"' \
    --level info 2>&1 | grep -v -E "SpringBoard|runningboardd|symptomsd|CommCenter|CloudKit|FrontBoard|UIKitCore|CoreFoundation" | tee "$SYSTEM_LOG" &
LOG_PID=$!

# Function to cleanup on exit
cleanup() {
    echo -e "\n🛑 Stopping log capture..."
    kill $LOG_PID 2>/dev/null || true
    echo "✅ Logs saved!"
}
trap cleanup EXIT

# Small delay to let log stream start
sleep 1

# Launch the app with console output attached
# This captures stdout/stderr including Console.WriteLine
xcrun simctl launch --console "$DEVICE_ID" com.urbanairship.richpush 2>&1 | tee "$CONSOLE_LOG"