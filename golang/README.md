# TCP Optimization - Go Implementation

## TCP Optimizations Applied

1. **TCP_NODELAY** - Disables Nagle's algorithm for low latency
2. **Socket Buffers** - 32KB read/write buffers
3. **KeepAlive** - Enabled with 30s period
4. **Timeouts** - 5 second timeout for client operations

## Requirements

- Go 1.20 or higher

## Build & Run

### Option 1: Using run.bat

```bash
run.bat
```

### Option 2: Manual

```bash
# Terminal 1 - Server
go run tcp_server.go

# Terminal 2 - Client (wait for server to start)
go run tcp_client.go
```

### Option 3: Build binaries

```bash
# Build
go build -o tcp_server.exe tcp_server.go
go build -o tcp_client.exe tcp_client.go

# Run
tcp_server.exe
tcp_client.exe
```

## Performance

Go's native TCP implementation with optimizations provides excellent performance with minimal overhead.

Expected performance: ~15,000-25,000 messages/second depending on system.
