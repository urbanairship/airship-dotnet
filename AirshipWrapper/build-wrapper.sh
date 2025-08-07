#!/bin/bash

# AirshipWrapper Build Script
# Creates a universal xcframework for iOS device and simulator

set -e  # Exit on any error
set -o pipefail  # Exit on pipe failures

echo "🧹 Cleaning previous builds..."

# Clean any existing build artifacts
rm -rf build/
rm -rf lib/AirshipWrapper.xcframework

# Create lib directory if it doesn't exist
mkdir -p lib

echo "🏗️  Building AirshipWrapper for iOS device..."
if command -v xcbeautify &> /dev/null; then
  xcodebuild -project AirshipWrapper.xcodeproj \
    -scheme AirshipWrapper \
    -configuration Release \
    -sdk iphoneos \
    -derivedDataPath build/DerivedData \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES 2>&1 | xcbeautify
else
  xcodebuild -project AirshipWrapper.xcodeproj \
    -scheme AirshipWrapper \
    -configuration Release \
    -sdk iphoneos \
    -derivedDataPath build/DerivedData \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES
fi

echo "🏗️  Building AirshipWrapper for iOS simulator..."
if command -v xcbeautify &> /dev/null; then
  xcodebuild -project AirshipWrapper.xcodeproj \
    -scheme AirshipWrapper \
    -configuration Release \
    -sdk iphonesimulator \
    -derivedDataPath build/DerivedData \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES 2>&1 | xcbeautify
else
  xcodebuild -project AirshipWrapper.xcodeproj \
    -scheme AirshipWrapper \
    -configuration Release \
    -sdk iphonesimulator \
    -derivedDataPath build/DerivedData \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES
fi

echo "📦 Creating xcframework..."
xcodebuild -create-xcframework \
  -framework build/DerivedData/Build/Products/Release-iphoneos/AirshipWrapper.framework \
  -framework build/DerivedData/Build/Products/Release-iphonesimulator/AirshipWrapper.framework \
  -output lib/AirshipWrapper.xcframework

echo "✅ AirshipWrapper.xcframework built successfully!"
echo "📍 Location: lib/AirshipWrapper.xcframework"

# Clean up intermediate build files but keep the xcframework
rm -rf build/

echo "🧹 Cleaned up intermediate build files"