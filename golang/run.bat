@echo off
echo ========================================
echo TCP Optimization Benchmark - Go
echo ========================================
echo.

echo [1/2] Starting server...
start "TCP Server (Go)" cmd /c "go run tcp_server.go"
timeout /t 2 /nobreak >nul

echo [2/2] Running client benchmark...
echo.
go run tcp_client.go

echo.
echo ========================================
echo Benchmark Complete!
echo ========================================
pause
