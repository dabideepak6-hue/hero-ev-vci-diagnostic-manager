@echo off
title DEEPAK DABI VCI SCAN - Local Bridge

color 0A

echo.
echo ==================================================
echo        DEEPAK DABI VCI SCAN
echo        HERO EV LOCAL VCI BRIDGE
echo ==================================================
echo.
echo Starting Local VCI Bridge...
echo.

cd /d "%~dp0"

if not exist "VidaVciBridge.csproj" (
    echo [ERROR] VidaVciBridge.csproj not found!
    echo.
    echo Please place this BAT file inside:
    echo bridge\
    echo.
    pause
    exit /b 1
)

where dotnet >nul 2>&1

if errorlevel 1 (
    echo [ERROR] .NET SDK is not installed or not in PATH.
    echo.
    echo Install .NET 8 SDK, then restart Windows.
    echo.
    pause
    exit /b 1
)

echo [1/3] Checking .NET...
dotnet --version

echo.
echo [2/3] Restoring project...
dotnet restore

if errorlevel 1 (
    echo.
    echo [ERROR] dotnet restore failed.
    pause
    exit /b 1
)

echo.
echo [3/3] Starting VCI Bridge...
echo.
echo Bridge URL:
echo http://127.0.0.1:8765
echo.
echo Test URL:
echo http://127.0.0.1:8765/api/status
echo.
echo Keep this window OPEN while using DEEPAK DABI VCI SCAN.
echo.
echo ==================================================
echo.

dotnet run

echo.
echo ==================================================
echo VCI Bridge stopped.
echo ==================================================
pause
