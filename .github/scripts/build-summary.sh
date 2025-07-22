#!/bin/bash

# Script to generate a build summary from logs

echo "::group::📊 Build Summary"

# Count warnings and errors
WARNING_COUNT=$(grep -i "warning" build.log 2>/dev/null | wc -l || echo "0")
ERROR_COUNT=$(grep -i "error" build.log 2>/dev/null | wc -l || echo "0")

# Extract build time
BUILD_TIME=$(grep -i "total time:" build.log 2>/dev/null | tail -1 || echo "Unknown")

# Create markdown summary for GitHub Actions
cat << EOF > $GITHUB_STEP_SUMMARY
## 📊 Build Summary

| Metric | Value |
|--------|-------|
| ⚠️ Warnings | $WARNING_COUNT |
| ❌ Errors | $ERROR_COUNT |
| ⏱️ Build Time | $BUILD_TIME |

### Key Issues
EOF

# Add top 5 warnings if any
if [ "$WARNING_COUNT" -gt 0 ]; then
    echo -e "\n#### Top Warnings:" >> $GITHUB_STEP_SUMMARY
    grep -i "warning" build.log | head -5 | while read -r line; do
        echo "- \`$line\`" >> $GITHUB_STEP_SUMMARY
    done
fi

# Add errors if any
if [ "$ERROR_COUNT" -gt 0 ]; then
    echo -e "\n#### Errors:" >> $GITHUB_STEP_SUMMARY
    grep -i "error" build.log | while read -r line; do
        echo "- \`$line\`" >> $GITHUB_STEP_SUMMARY
    done
fi

echo "::endgroup::"