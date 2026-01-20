#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}🤖 Airship .NET MAUI Android Sample Runner${NC}"
echo "==========================================="

# Parse command line arguments
FAST_BUILD=false
if [[ "$1" == "--fast" ]]; then
    FAST_BUILD=true
    echo "🚀 Fast build requested - skipping clean"
fi

# Check if we're in the right directory
if [ ! -f "MauiSample.csproj" ]; then
    echo -e "${RED}❌ Error: MauiSample.csproj not found. Please run this script from the MauiSample directory.${NC}"
    exit 1
fi

# Check UseProjectReferences setting
USE_PROJECT_REFS=$(grep -o '<UseProjectReferences>.*</UseProjectReferences>' MauiSample.csproj | sed 's/<[^>]*>//g')

if [ "$USE_PROJECT_REFS" = "true" ]; then
    echo -e "${GREEN}✅ Using project references (development mode)${NC}"
    echo "   SDK changes will be built automatically with the sample app"
else
    echo -e "${YELLOW}⚠️  Using NuGet packages${NC}"
    echo "   To use project references for development, set UseProjectReferences=true in MauiSample.csproj"
    
    # Build the native bindings first if using packages
    echo -e "${YELLOW}🔨 Building native Android bindings...${NC}"
    cd ..
    if ! ./gradlew :binderator:build; then
        echo -e "${RED}❌ Error: Failed to build native bindings${NC}"
        exit 1
    fi
    echo -e "${GREEN}✅ Native bindings built successfully${NC}"
    
    # Return to MauiSample directory
    cd MauiSample
fi

# Check if any device is connected
if ! adb devices | grep -q "device$"; then
    echo -e "${RED}❌ No Android device detected. Please start an emulator or connect a device.${NC}"
    exit 1
fi

# Clean previous builds (skip if --fast)
if [[ "$FAST_BUILD" != "true" ]]; then
    echo -e "${YELLOW}🧹 Cleaning previous builds...${NC}"
    dotnet clean -f net10.0-android
fi

# Build the app
echo -e "${YELLOW}🔨 Building Android APK...${NC}"
if ! dotnet build -f net10.0-android -p:NoWarn=NU1608 -v quiet --nologo; then
    echo -e "${RED}❌ Error: Failed to build Android APK${NC}"
    exit 1
fi

# Uninstall existing app to ensure clean state
echo -e "${YELLOW}🗑️  Uninstalling existing app...${NC}"
adb uninstall com.urbanairship.sample 2>/dev/null || true

# Install the new APK
echo -e "${YELLOW}📦 Installing APK...${NC}"
if ! adb install -r bin/Debug/net10.0-android/com.urbanairship.sample-Signed.apk; then
    echo -e "${RED}❌ Error: Failed to install APK${NC}"
    exit 1
fi

# Clear previous logs
adb logcat -c

echo -e "${YELLOW}🚀 Launching app...${NC}"

# Launch the app - Try multiple methods
echo "Attempting to launch app..."

# Method 1: Try with monkey (most common)
if adb shell monkey -p com.urbanairship.sample -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1; then
    echo -e "${GREEN}✅ App launched successfully with monkey${NC}"
else
    # Method 2: Try with am start using common MAUI activity name
    if adb shell am start -n com.urbanairship.sample/crc64ffe6b59f69e5ae4a.MainActivity > /dev/null 2>&1; then
        echo -e "${GREEN}✅ App launched successfully with am start${NC}"
    else
        # Method 3: Try launching by package name only
        if adb shell am start --user 0 $(adb shell cmd package resolve-activity --brief com.urbanairship.sample | tail -n 1) > /dev/null 2>&1; then
            echo -e "${GREEN}✅ App launched successfully${NC}"
        else
            echo -e "${YELLOW}⚠️  Could not auto-launch app. Please launch it manually from the device.${NC}"
            echo -e "${GREEN}App is installed and ready to use.${NC}"
        fi
    fi
fi

echo -e "${GREEN}🎉 App deployed and running successfully!${NC}"
echo ""
echo -e "${GREEN}📋 Streaming device logs (Press Ctrl+C to stop)...${NC}"
echo "========================================"
echo ""

# Create log directory with PST timestamp
LOG_BASE_DIR="Sample Run Logs"
mkdir -p "$LOG_BASE_DIR"

# Get PST timestamp (TZ=America/Los_Angeles ensures PST/PDT)
PST_TIMESTAMP=$(TZ=America/Los_Angeles date +"%Y-%m-%d_%H-%M-%S_PST")
LOG_DIR="$LOG_BASE_DIR/$PST_TIMESTAMP"
mkdir -p "$LOG_DIR"

# Create log file
LOG_FILE="$LOG_DIR/logcat.log"

echo -e "${YELLOW}📋 Logging to directory: $LOG_DIR${NC}"
echo -e "${YELLOW}   Device logs: logcat.log${NC}"
echo ""

# Stream logs with filtering for our app and Airship
# - Using Info level (I) instead of Verbose (V) for less noise
# - brief format for cleaner output
# - AndroidRuntime:E to catch crashes
# Tee to both console and file, with colors added for terminal
RESET='\033[0m'
adb logcat -v brief com.urbanairship.sample:I Airship:I UALib:I AndroidRuntime:E *:S 2>&1 | \
    while IFS= read -r line; do
        case "$line" in
            I/*) echo -e "${GREEN}${line}${RESET}" ;;
            D/*) echo -e "\033[0;36m${line}${RESET}" ;;
            W/*) echo -e "${YELLOW}${line}${RESET}" ;;
            E/*) echo -e "${RED}${line}${RESET}" ;;
            *)   echo "$line" ;;
        esac
    done | tee "$LOG_FILE"