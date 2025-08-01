#!/bin/bash
set -e
set -x

SCRIPT_DIRECTORY="$(dirname "$0")"
ROOT_PATH=$SCRIPT_DIRECTORY/../

# First argument is always the version
VERSION=$1
shift

# Process remaining arguments as named parameters
while [[ $# -gt 0 ]]; do
  case $1 in
    --ios)
      IOS_VERSION="$2"
      shift 2
      ;;
    --android)
      ANDROID_VERSION="$2"
      shift 2
      ;;
    *)
      echo "Unknown parameter: $1"
      exit 1
      ;;
  esac
done

if [ -z "$VERSION" ]; then
    echo "Error: Version is required"
    echo "Usage: $0 <version> [--ios <ios_version>] [--android <android_version>]"
    echo "Example: $0 20.1.0 --ios 19.7.0 --android 19.9.0"
    exit 1
fi

RELEASE_DATE=$(date +"%B %-d, %Y")

# Determine release type based on version
if [[ $VERSION =~ \.0\.0$ ]]; then
    RELEASE_TYPE="Major"
elif [[ $VERSION =~ \.0$ ]]; then
    RELEASE_TYPE="Minor"
else
    RELEASE_TYPE="Patch"
fi

# Create changelog entry
NEW_ENTRY="## Version $VERSION - $RELEASE_DATE\n"

if [ -n "$IOS_VERSION" ] || [ -n "$ANDROID_VERSION" ]; then
    NEW_ENTRY+="$RELEASE_TYPE release that updates"

    if [ -n "$ANDROID_VERSION" ]; then
        NEW_ENTRY+=" the Android SDK to $ANDROID_VERSION"
    fi

    if [ -n "$IOS_VERSION" ] && [ -n "$ANDROID_VERSION" ]; then
        NEW_ENTRY+=" and"
    fi

    if [ -n "$IOS_VERSION" ]; then
        NEW_ENTRY+=" the iOS SDK to $IOS_VERSION"
    fi

    NEW_ENTRY+=".\n\n### Changes\n"

    if [ -n "$IOS_VERSION" ]; then
        NEW_ENTRY+="- Updated iOS SDK to $IOS_VERSION\n"
    fi

    if [ -n "$ANDROID_VERSION" ]; then
        NEW_ENTRY+="- Updated Android SDK to $ANDROID_VERSION\n"
    fi
else
    NEW_ENTRY+="$RELEASE_TYPE release.\n\n### Changes\n- \n"
fi

# Also update SDK versions in airship.properties if provided
if [ -n "$IOS_VERSION" ]; then
    sed -i '' "s/^iosVersion = .*/iosVersion = $IOS_VERSION/g" "$ROOT_PATH/airship.properties"
    # Update the iOS framework zip filename
    sed -i '' "s/^iosFrameworkZip = .*/iosFrameworkZip = Airship-iOS-SDK-$IOS_VERSION.zip/g" "$ROOT_PATH/airship.properties"
fi

if [ -n "$ANDROID_VERSION" ]; then
    sed -i '' "s/^androidVersion = .*/androidVersion = $ANDROID_VERSION/g" "$ROOT_PATH/airship.properties"
fi

# Create temporary file with new content
TEMP_FILE=$(mktemp)

# Add the header line
echo "# Airship DotNet Changelog" > "$TEMP_FILE"
echo "" >> "$TEMP_FILE"
echo -e "$NEW_ENTRY" >> "$TEMP_FILE"

# Append the rest of the existing changelog (skipping the header and empty line)
tail -n +3 "$ROOT_PATH/CHANGELOG.md" >> "$TEMP_FILE"

# Replace original file with new content
mv "$TEMP_FILE" "$ROOT_PATH/CHANGELOG.md"

echo "Changelog updated with version $VERSION"
if [ -n "$IOS_VERSION" ] || [ -n "$ANDROID_VERSION" ]; then
    echo "SDK versions updated in airship.properties"
fi