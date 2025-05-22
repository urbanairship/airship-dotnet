#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}🤖 Airship .NET MAUI Android Sample Runner${NC}"
echo "==========================================="

# Check if we're in the right directory
if [ ! -f "MauiSample.csproj" ]; then
    echo -e "${RED}❌ Error: MauiSample.csproj not found. Please run this script from the MauiSample directory.${NC}"
    exit 1
fi

# Build the native bindings first
echo -e "${YELLOW}🔨 Building native Android bindings...${NC}"
cd ..
if ! ./gradlew :binderator:build; then
    echo -e "${RED}❌ Error: Failed to build native bindings${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Native bindings built successfully${NC}"

# Return to MauiSample directory
cd MauiSample

# Check if any device is connected
if ! adb devices | grep -q "device$"; then
    echo -e "${RED}❌ No Android device detected. Please start an emulator or connect a device.${NC}"
    exit 1
fi

# Build the app
echo -e "${YELLOW}🔨 Building Android APK...${NC}"
if ! ~/.dotnet/dotnet build -f net8.0-android; then
    echo -e "${RED}❌ Error: Failed to build Android APK${NC}"
    exit 1
fi

# Install the new APK
echo -e "${YELLOW}📦 Installing APK...${NC}"
if ! adb install -r bin/Debug/net8.0-android/com.urbanairship.sample-Signed.apk; then
    echo -e "${RED}❌ Error: Failed to install APK${NC}"
    exit 1
fi

echo -e "${YELLOW}🚀 Launching app...${NC}"
if ! adb shell monkey -p com.urbanairship.sample -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1; then
    echo -e "${RED}❌ Error: Failed to launch app${NC}"
    exit 1
fi

echo -e "${GREEN}🎉 App deployed and running successfully!${NC}"