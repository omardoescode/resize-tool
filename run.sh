#!/bin/bash


# BEFORE U RUN:
#   do `chmod +x run-resize.sh` to make the script executable 

# Check if argument is provided
if [ -z "$1" ]; then
    echo "Usage: ./run-resize.sh <image_name.png>"
    echo "Example: ./run-resize.sh test.png"
    exit 1
fi

# Set base paths
EXE_PATH="bin/Debug/net9.0/resize-tool"
RESOURCES_DIR="bin/Debug/net9.0/resources"

# Check if the executable exists
if [ ! -f "$EXE_PATH" ]; then
    echo "Error: Could not find '$EXE_PATH'. Build the project first."
    exit 1
fi

# Check if the image exists
if [ ! -f "$RESOURCES_DIR/$1" ]; then
    echo "Error: Image '$RESOURCES_DIR/$1' not found."
    echo "Available images:"
    ls "$RESOURCES_DIR"/*.{png,jpg} 2>/dev/null || echo "(No images found)"
    exit 1
fi

# Run the tool with the full image path
"$EXE_PATH" "$RESOURCES_DIR/$1"