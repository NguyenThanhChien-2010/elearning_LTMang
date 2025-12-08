@echo off
echo ========================================
echo Building C# TCP Optimization Project
echo ========================================

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 6.0 SDK or later from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Building project...
dotnet build TcpOptimization.csproj -c Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo To run:
echo   Server: dotnet run --project TcpOptimization.csproj -- server
echo   Client: dotnet run --project TcpOptimization.csproj -- client
echo.
pause
