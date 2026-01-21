#!/bin/bash
# Thin xcframeworks to remove unsupported platforms
# Removes visionOS (xros) slices - not a supported .NET TFM
# Keeps iOS, macCatalyst, tvOS - all officially supported .NET TFMs

set -e

CARTHAGE_BUILD_DIR="${1:-Carthage/Build}"

if [ ! -d "$CARTHAGE_BUILD_DIR" ]; then
    echo "ERROR: Carthage build directory not found: $CARTHAGE_BUILD_DIR"
    exit 1
fi

echo "Removing visionOS (xros) slices from xcframeworks in $CARTHAGE_BUILD_DIR..."

for xcframework in "$CARTHAGE_BUILD_DIR"/*.xcframework; do
    if [ -d "$xcframework" ]; then
        framework_name=$(basename "$xcframework")

        # Remove visionOS slices
        for slice in "$xcframework"/*/; do
            slice_name=$(basename "$slice")
            case "$slice_name" in
                xros-*)
                    echo "  Removing $slice_name from $framework_name"
                    rm -rf "$slice"
                    ;;
            esac
        done

        # Update Info.plist to remove visionOS entries
        plist="$xcframework/Info.plist"
        if [ -f "$plist" ]; then
            /usr/libexec/PlistBuddy -c "Print :AvailableLibraries" "$plist" > /dev/null 2>&1 || continue

            count=$(/usr/libexec/PlistBuddy -c "Print :AvailableLibraries" "$plist" 2>/dev/null | grep -c "Dict" || echo "0")

            for ((i=count-1; i>=0; i--)); do
                platform=$(/usr/libexec/PlistBuddy -c "Print :AvailableLibraries:$i:SupportedPlatform" "$plist" 2>/dev/null || echo "")

                if [ "$platform" = "xros" ]; then
                    echo "  Removing plist entry for xros from $framework_name"
                    /usr/libexec/PlistBuddy -c "Delete :AvailableLibraries:$i" "$plist" 2>/dev/null || true
                fi
            done
        fi
    fi
done

echo ""
echo "Done! Remaining sizes:"
du -sh "$CARTHAGE_BUILD_DIR"/*.xcframework | sort -h
