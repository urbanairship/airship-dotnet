#!/bin/bash

# Build AirshipWrapper.xcframework after Carthage dependencies are built

echo "Building AirshipWrapper.xcframework..."

cd AirshipWrapper

# Build for iOS device
xcodebuild -scheme AirshipWrapper -configuration Release -sdk iphoneos -archivePath build/ios.xcarchive archive SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

# Build for iOS simulator
xcodebuild -scheme AirshipWrapper -configuration Release -sdk iphonesimulator -archivePath build/simulator.xcarchive archive SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

# Create xcframework
xcodebuild -create-xcframework \
    -framework build/ios.xcarchive/Products/Library/Frameworks/AirshipWrapper.framework \
    -framework build/simulator.xcarchive/Products/Library/Frameworks/AirshipWrapper.framework \
    -output ../Carthage/Build/AirshipWrapper.xcframework

# Clean up
rm -rf build/

echo "AirshipWrapper.xcframework built successfully!"