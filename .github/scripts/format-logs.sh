#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to format and filter build output
format_build_output() {
    # Use sed and awk to format the output
    while IFS= read -r line; do
        # Skip verbose MSBuild output
        if [[ "$line" =~ "Determining projects to restore" ]] || 
           [[ "$line" =~ "Restored.*\.csproj" ]] ||
           [[ "$line" =~ "Time Elapsed" ]]; then
            continue
        fi
        
        # Highlight errors in red
        if [[ "$line" =~ "error" ]] || [[ "$line" =~ "ERROR" ]]; then
            echo -e "${RED}❌ $line${NC}"
        # Highlight warnings in yellow
        elif [[ "$line" =~ "warning" ]] || [[ "$line" =~ "WARNING" ]]; then
            echo -e "${YELLOW}⚠️  $line${NC}"
        # Highlight success in green
        elif [[ "$line" =~ "BUILD SUCCESSFUL" ]] || [[ "$line" =~ "succeeded" ]]; then
            echo -e "${GREEN}✅ $line${NC}"
        # Highlight important tasks in blue
        elif [[ "$line" =~ "Task :" ]] || [[ "$line" =~ "> Task" ]]; then
            echo -e "${BLUE}▶️  $line${NC}"
        # Skip overly verbose lines
        elif [[ "$line" =~ "Microsoft (R) Build Engine" ]] ||
             [[ "$line" =~ "Copyright (C)" ]] ||
             [[ "$line" =~ "Loading projects" ]] ||
             [[ "$line" =~ "Included response file" ]]; then
            continue
        else
            # Output other lines normally
            echo "$line"
        fi
    done
}

# Execute the command passed as arguments and format output
"$@" 2>&1 | format_build_output