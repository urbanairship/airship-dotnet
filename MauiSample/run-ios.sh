#!/bin/bash

# Build and run Airship .NET MAUI iOS sample app
# Works for any user on macOS

set -e  # Exit on any error

# Get the script directory to find the project
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR"

# Ensure .NET is in PATH
export PATH="$HOME/.dotnet:$PATH"

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

# Check if iOS Simulator is running
if ! xcrun simctl list devices | grep -q "(Booted)"; then
    echo "❌ Error: No iOS Simulator is currently running"
    echo "Please start an iOS Simulator first"
    exit 1
fi

echo "🏗️  Building iOS app..."
cd "$PROJECT_DIR"

# Build the app
dotnet build -f net8.0-ios \
    -p:RuntimeIdentifier=iossimulator-arm64 \
    -p:CodesignProvision="dotnet-maui-sample-profile"

if [[ $? -ne 0 ]]; then
    echo "❌ Build failed. Make sure you have:"
    echo "   1. .NET 8 SDK installed"
    echo "   2. iOS workloads installed (dotnet workload install ios)"
    echo "   3. Provisioning profile 'dotnet-maui-sample-profile' available"
    exit 1
fi

echo "📱 Installing app to simulator..."
xcrun simctl install booted "$PROJECT_DIR/bin/Debug/net8.0-ios/iossimulator-arm64/MauiSample.app"

echo "🚀 Launching app..."
xcrun simctl launch booted com.urbanairship.richpush

echo "✅ App launched successfully!"