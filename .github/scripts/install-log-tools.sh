#!/bin/bash

# Install log formatting tools
echo "Installing log formatting tools..."

# Check if running in CI
if [ -n "$CI" ]; then
    # Install xcbeautify for Xcode/iOS build logs
    if ! command -v xcbeautify &> /dev/null; then
        echo "Installing xcbeautify..."
        brew install xcbeautify
    fi
    
    # Install xcpretty as a fallback
    if ! command -v xcpretty &> /dev/null; then
        echo "Installing xcpretty..."
        gem install xcpretty
    fi
fi

echo "Log tools installation complete!"