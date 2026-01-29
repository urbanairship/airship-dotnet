#!/bin/bash
set -e

SCRIPT_DIRECTORY="$(dirname "$0")"
ROOT_PATH="$SCRIPT_DIRECTORY/.."

VERSION=""
PROXY_VERSION=""

usage() {
    echo "Usage: $0 <version> [-p <proxy_version>]"
    echo ""
    echo "Updates the Airship .NET library version."
    echo ""
    echo "Arguments:"
    echo "  <version>           The .NET library version (e.g., 20.2.1)"
    echo ""
    echo "Options:"
    echo "  -p <proxy_version>  Also update the native SDK versions (iOS and Android)"
    echo "                      to the specified proxy/SDK version (e.g., 19.1.0)"
    echo ""
    echo "Examples:"
    echo "  $0 20.2.1                    # Update .NET lib version only"
    echo "  $0 20.2.1 -p 19.1.0          # Update .NET lib and native SDK versions"
    exit 1
}

# Parse arguments
if [ -z "$1" ]; then
    usage
fi

VERSION=$1
shift

while getopts "p:" opt; do
    case $opt in
        p)
            PROXY_VERSION=$OPTARG
            ;;
        *)
            usage
            ;;
    esac
done

echo "Updating Airship .NET library version to $VERSION"

# Update crossPlatformVersion in airship.properties
sed -i '' "s/^crossPlatformVersion = .*/crossPlatformVersion = $VERSION/g" "$ROOT_PATH/airship.properties"

# Update SharedAssemblyInfo.Common.cs
sed -i '' "s/\[assembly: UACrossPlatformVersion (\"[^\"]*\")\]/[assembly: UACrossPlatformVersion (\"$VERSION\")]/g" "$ROOT_PATH/src/SharedAssemblyInfo.Common.cs"

# Update Airship.Version in iOS Airship.cs
sed -i '' "s/public static string Version => \"[^\"]*\";/public static string Version => \"$VERSION\";/g" "$ROOT_PATH/src/Airship.Net/Platforms/iOS/Airship.cs"

# Update Airship.Version in Android Airship.cs
sed -i '' "s/public static string Version => \"[^\"]*\";/public static string Version => \"$VERSION\";/g" "$ROOT_PATH/src/Airship.Net/Platforms/Android/Airship.cs"

# Update version references in README.md
sed -i '' "s/\(Airship\.Net[^:]*:\s*\)[0-9]\+\.[0-9]\+\.[0-9]\+/\1$VERSION/g" "$ROOT_PATH/README.md" 2>/dev/null || true

# Update version in CLAUDE.md if it exists
if [ -f "$ROOT_PATH/CLAUDE.md" ]; then
    sed -i '' "s/\(crossPlatformVersion[^0-9]*\)[0-9]\+\.[0-9]\+\.[0-9]\+/\1$VERSION/g" "$ROOT_PATH/CLAUDE.md"
fi

echo "✓ Updated .NET library version to $VERSION"

# Update native SDK versions if -p flag was provided
if [ -n "$PROXY_VERSION" ]; then
    echo ""
    echo "Updating native SDK versions to $PROXY_VERSION"

    # Update iosVersion in airship.properties
    sed -i '' "s/^iosVersion = .*/iosVersion = $PROXY_VERSION/g" "$ROOT_PATH/airship.properties"

    # Update androidVersion in airship.properties
    sed -i '' "s/^androidVersion = .*/androidVersion = $PROXY_VERSION/g" "$ROOT_PATH/airship.properties"

    # Update iosFrameworkZip in airship.properties
    sed -i '' "s/^iosFrameworkZip = .*/iosFrameworkZip = Airship-iOS-SDK-$PROXY_VERSION.zip/g" "$ROOT_PATH/airship.properties"

    echo "✓ Updated iOS SDK version to $PROXY_VERSION"
    echo "✓ Updated Android SDK version to $PROXY_VERSION"
    echo "✓ Updated iOS framework zip to Airship-iOS-SDK-$PROXY_VERSION.zip"
fi

echo ""
echo "Don't forget to:"
echo "  1. Update CHANGELOG.md with release notes"
echo "  2. Run ./gradlew build to sync versions"
echo "  3. Commit the changes"
