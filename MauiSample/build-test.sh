#!/bin/bash
cd /Users/david.crow/source/airship-dotnet/MauiSample
echo "Starting build..."
~/.dotnet/dotnet build -f net8.0-android 2>&1 | tee build-output.log
echo "Build completed. Check build-output.log for details."