@echo off
echo ========================================
echo Node.js TCP Optimization Project
echo ========================================

REM Check if Node.js is installed
node --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: Node.js not found!
    echo Please install Node.js 14+ from: https://nodejs.org/
    pause
    exit /b 1
)

echo Node.js is installed.
echo Installing dependencies...
npm install
if %ERRORLEVEL% neq 0 (
    echo WARNING: npm install failed, but no external dependencies are required.
)

echo.
echo ========================================
echo Setup completed!
echo ========================================
echo.
echo To run:
echo   Server: npm run server  OR  node tcp_server.js
echo   Client: npm run client  OR  node tcp_client.js
echo.
pause
