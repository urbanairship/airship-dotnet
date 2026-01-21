#!/bin/bash
# Thin xcframeworks to iOS-only (device + simulator)
# Removes tvOS, visionOS, macCatalyst slices to reduce NuGet package size

set -e

CARTHAGE_BUILD_DIR="${1:-Carthage/Build}"

if [ ! -d "$CARTHAGE_BUILD_DIR" ]; then
    echo "ERROR: Carthage build directory not found: $CARTHAGE_BUILD_DIR"
    exit 1
fi

echo "Thinning xcframeworks in $CARTHAGE_BUILD_DIR to iOS-only..."

for xcframework in "$CARTHAGE_BUILD_DIR"/*.xcframework; do
    if [ -d "$xcframework" ]; then
        framework_name=$(basename "$xcframework")
        echo "Processing $framework_name..."

        # Remove non-iOS slices
        for slice in "$xcframework"/*/; do
            slice_name=$(basename "$slice")
            case "$slice_name" in
                ios-arm64|ios-arm64_x86_64-simulator|_CodeSignature|Info.plist)
                    # Keep iOS device, simulator, and metadata
                    ;;
                *)
                    echo "  Removing $slice_name"
                    rm -rf "$slice"
                    ;;
            esac
        done

        # Update Info.plist to remove references to deleted slices
        plist="$xcframework/Info.plist"
        if [ -f "$plist" ]; then
            # Create a filtered plist with only iOS entries
            /usr/libexec/PlistBuddy -c "Print :AvailableLibraries" "$plist" > /dev/null 2>&1 || continue

            # Get the count of libraries
            count=$(/usr/libexec/PlistBuddy -c "Print :AvailableLibraries" "$plist" 2>/dev/null | grep -c "Dict" || echo "0")

            # Iterate backwards to safely remove entries
            for ((i=count-1; i>=0; i--)); do
                platform=$(/usr/libexec/PlistBuddy -c "Print :AvailableLibraries:$i:SupportedPlatform" "$plist" 2>/dev/null || echo "")
                variant=$(/usr/libexec/PlistBuddy -c "Print :AvailableLibraries:$i:SupportedPlatformVariant" "$plist" 2>/dev/null || echo "")

                # Keep only iOS device and simulator (remove tvOS, visionOS, macCatalyst)
                if [ "$platform" != "ios" ] || [ "$variant" = "maccatalyst" ]; then
                    echo "  Removing plist entry for $platform ($variant)"
                    /usr/libexec/PlistBuddy -c "Delete :AvailableLibraries:$i" "$plist" 2>/dev/null || true
                fi
            done
        fi
    fi
done

echo "Done! Checking sizes:"
du -sh "$CARTHAGE_BUILD_DIR"/*.xcframework | sort -h
