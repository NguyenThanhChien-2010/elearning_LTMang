@echo off
echo ========================================
echo Python TCP Optimization Project
echo ========================================

REM Check if Python is installed
python --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: Python not found!
    echo Please install Python 3.7+ from: https://www.python.org/downloads/
    pause
    exit /b 1
)

echo Python is installed.
echo.
echo Usage:
echo   Server: python tcp_server.py
echo   Client: python tcp_client.py
echo.
echo Ready to run!
echo.
pause
