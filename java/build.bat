@echo off
echo ========================================
echo Building Java TCP Optimization Project
echo ========================================

REM Check if Java is installed
java -version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: Java not found!
    echo Please install JDK 8+ from: https://adoptium.net/
    pause
    exit /b 1
)

REM Check if javac is available
javac -version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: Java compiler (javac) not found!
    echo Please ensure JDK is installed and added to PATH
    pause
    exit /b 1
)

echo Compiling Java files for Java 8...
javac -source 1.8 -target 1.8 TcpServer.java TcpClient.java
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
echo   Server: java TcpServer
echo   Client: java TcpClient
echo.
pause
