@echo off
setlocal

:: Check if argument is provided
if "%~1"=="" (
    echo Usage: run-resize.cmd "image_name.png"
    echo Example: run-resize.cmd test.png
    exit /b 1
)

:: Set base paths
set EXE_PATH=bin\Debug\net9.0\resize-tool.exe
set RESOURCES_DIR=resources

:: Check if the executable exists
if not exist "%EXE_PATH%" (
    echo Error: Could not find "%EXE_PATH%". Build the project first.
    exit /b 1
)

:: Check if the image exists
if not exist "%RESOURCES_DIR%\%~1" (
    echo Error: Image "%RESOURCES_DIR%\%~1" not found.
    echo Available images:
    dir /b "%RESOURCES_DIR%\*.png" "%RESOURCES_DIR%\*.jpg" 2>nul
    exit /b 1
)

:: Run the tool with the full image path
"%EXE_PATH%" "%RESOURCES_DIR%\%~1"

endlocal