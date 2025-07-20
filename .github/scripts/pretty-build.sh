#!/bin/bash

# Pretty build wrapper script that formats output from various build tools

# ANSI color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
BOLD='\033[1m'
DIM='\033[2m'
NC='\033[0m' # No Color

# Function to print section headers
print_header() {
    echo -e "\n${BOLD}${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${BOLD}${BLUE}  $1${NC}"
    echo -e "${BOLD}${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}\n"
}

# Function to format gradle output
format_gradle_output() {
    local task_pattern='> Task :'
    local error_pattern='(ERROR|FAILED|error:|Error:)'
    local warning_pattern='(WARNING|warning:|Warning:)'
    local success_pattern='(BUILD SUCCESSFUL|succeeded)'
    
    while IFS= read -r line; do
        # Skip empty lines
        [[ -z "$line" ]] && continue
        
        # Task headers
        if [[ "$line" =~ $task_pattern ]]; then
            echo -e "${CYAN}▶ ${line}${NC}"
        # Errors
        elif [[ "$line" =~ $error_pattern ]]; then
            echo -e "${RED}✗ ${line}${NC}"
        # Warnings
        elif [[ "$line" =~ $warning_pattern ]]; then
            echo -e "${YELLOW}⚠ ${line}${NC}"
        # Success messages
        elif [[ "$line" =~ $success_pattern ]]; then
            echo -e "${GREEN}✓ ${line}${NC}"
        # Dim verbose output
        elif [[ "$line" =~ ^(Download|Downloading|Resolving|Resolved) ]]; then
            echo -e "${DIM}${line}${NC}"
        else
            echo "$line"
        fi
    done
}

# Function to run command with formatted output
run_with_format() {
    local cmd="$1"
    local formatter="$2"
    
    if [ -z "$formatter" ]; then
        # No formatter specified, use generic formatting
        $cmd 2>&1 | format_gradle_output
    else
        # Use specified formatter
        case "$formatter" in
            "xcbeautify")
                if command -v xcbeautify &> /dev/null; then
                    $cmd 2>&1 | xcbeautify
                else
                    $cmd 2>&1 | format_gradle_output
                fi
                ;;
            "xcpretty")
                if command -v xcpretty &> /dev/null; then
                    $cmd 2>&1 | xcpretty --color
                else
                    $cmd 2>&1 | format_gradle_output
                fi
                ;;
            *)
                $cmd 2>&1 | format_gradle_output
                ;;
        esac
    fi
    
    return ${PIPESTATUS[0]}
}

# Main execution
case "$1" in
    "gradle")
        shift
        print_header "Gradle Build"
        run_with_format "./gradlew $*" "gradle"
        ;;
    "xcode")
        shift
        print_header "Xcode Build"
        run_with_format "xcodebuild $*" "xcbeautify"
        ;;
    "dotnet")
        shift
        print_header ".NET Build"
        run_with_format "dotnet $*" "dotnet"
        ;;
    *)
        # Default: run command as-is with generic formatting
        run_with_format "$*" ""
        ;;
esac

exit_code=$?

# Print summary
if [ $exit_code -eq 0 ]; then
    echo -e "\n${GREEN}${BOLD}✅ Build completed successfully!${NC}"
else
    echo -e "\n${RED}${BOLD}❌ Build failed with exit code $exit_code${NC}"
fi

exit $exit_code