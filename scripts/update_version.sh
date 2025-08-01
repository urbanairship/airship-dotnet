#!/bin/bash -ex
SCRIPT_DIRECTORY="$(dirname "$0")"
ROOT_PATH=$SCRIPT_DIRECTORY/../

# The version supplied as first argument
VERSION=$1

if [ -z "$1" ]; then
    echo "No version supplied"
    echo "Usage: $0 <version>"
    echo "Example: $0 20.1.0"
    exit 1
fi

# Update crossPlatformVersion in airship.properties
sed -i '' "s/^crossPlatformVersion = .*/crossPlatformVersion = $VERSION/g" "$ROOT_PATH/airship.properties"

# Update version references in documentation
sed -i '' "s/\(Airship\.Net[^:]*:\s*\)[0-9]\+\.[0-9]\+\.[0-9]\+/\1$VERSION/g" "$ROOT_PATH/README.md" 2>/dev/null || true

# Update version in CLAUDE.md if it exists
if [ -f "$ROOT_PATH/CLAUDE.md" ]; then
    sed -i '' "s/\(crossPlatformVersion[^0-9]*\)[0-9]\+\.[0-9]\+\.[0-9]\+/\1$VERSION/g" "$ROOT_PATH/CLAUDE.md"
fi

echo "Version updated to $VERSION"
echo ""
echo "Don't forget to:"
echo "  1. Update CHANGELOG.md with release notes"
echo "  2. Run ./gradlew build to sync versions"
echo "  3. Commit the changes"