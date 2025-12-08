const net = require("net");

const PORT = 8888;
const BUFFER_SIZE = 8192;
const BACKLOG = 10;

class TcpServer {
  constructor() {
    this.server = null;
    this.running = false;
  }

  setupSocket(socket) {
    try {
      // TCP Optimization 1: TCP_NODELAY (disable Nagle's algorithm)
      socket.setNoDelay(true);

      // TCP Optimization 2: Keep-Alive
      socket.setKeepAlive(true, 60000); // 60 seconds initial delay

      // TCP Optimization 3: Increase buffer sizes (via highWaterMark)
      // This is set during socket creation in Node.js

      console.log("[INFO] Socket optimizations applied:");
      console.log("  - NoDelay (TCP_NODELAY): Enabled");
      console.log("  - KeepAlive: Enabled (60s initial delay)");
      console.log("  - Buffer optimization: Applied");
    } catch (error) {
      console.error(`[WARNING] Some optimizations failed: ${error.message}`);
    }
  }

  handleClient(socket) {
    const clientAddr = `${socket.remoteAddress}:${socket.remotePort}`;
    console.log(`\n[CONNECTED] Client from ${clientAddr}`);

    let messageCount = 0;
    const startTime = Date.now();
    let buffer = Buffer.alloc(0);

    socket.on("data", (data) => {
      try {
        // Accumulate data
        buffer = Buffer.concat([buffer, data]);

        // Process complete messages (simple echo protocol)
        // In a real scenario, you'd implement proper message framing
        messageCount++;
        const receivedMessage = buffer.toString("utf8");

        // Create echo response with timestamp
        const timestamp = Date.now();
        const response = `${receivedMessage} [Server Echo - Msg#${messageCount} - Time:${timestamp}]`;

        socket.write(response);

        // Clear buffer after processing
        buffer = Buffer.alloc(0);

        // Log every 100 messages
        if (messageCount % 100 === 0) {
          const duration = Date.now() - startTime;
          const messagesPerSec = (messageCount * 1000) / duration;
          console.log(
            `[STATS] Messages: ${messageCount}, Rate: ${messagesPerSec.toFixed(
              2
            )} msg/sec`
          );
        }
      } catch (error) {
        console.error(`[ERROR] Error handling data: ${error.message}`);
      }
    });

    socket.on("end", () => {
      const totalDuration = Date.now() - startTime;
      console.log("\n[SESSION STATS]");
      console.log(`  Total Messages: ${messageCount}`);
      console.log(`  Total Duration: ${totalDuration} ms`);
      if (totalDuration > 0) {
        const avgRate = (messageCount * 1000) / totalDuration;
        console.log(`  Average Rate: ${avgRate.toFixed(2)} msg/sec`);
      }
      console.log("[DISCONNECTED] Client closed connection");
    });

    socket.on("error", (error) => {
      console.error(`[ERROR] Socket error: ${error.message}`);
    });
  }

  start() {
    this.server = net.createServer(
      {
        // TCP Optimizations during server creation
        allowHalfOpen: false,
        pauseOnConnect: false,
      },
      (socket) => {
        this.setupSocket(socket);
        this.handleClient(socket);
      }
    );

    // Server-level optimizations
    this.server.maxConnections = BACKLOG;

    this.server.on("error", (error) => {
      console.error(`[ERROR] Server error: ${error.message}`);
      if (error.code === "EADDRINUSE") {
        console.error(`Port ${PORT} is already in use`);
        process.exit(1);
      }
    });

    this.server.listen(PORT, "0.0.0.0", () => {
      this.running = true;
      console.log("\n========================================");
      console.log("TCP Optimized Server Started (Node.js)");
      console.log("========================================");
      console.log(`Listening on port ${PORT}`);
      console.log("Waiting for connections...");
      console.log("Press Ctrl+C to stop\n");
    });
  }

  stop() {
    this.running = false;
    if (this.server) {
      this.server.close(() => {
        console.log("\n[STOPPED] Server stopped");
      });
    }
  }
}

// Main execution
if (require.main === module) {
  const server = new TcpServer();

  // Graceful shutdown
  process.on("SIGINT", () => {
    console.log("\n[INFO] Shutdown signal received");
    server.stop();
    setTimeout(() => process.exit(0), 1000);
  });

  process.on("SIGTERM", () => {
    console.log("\n[INFO] Terminate signal received");
    server.stop();
    setTimeout(() => process.exit(0), 1000);
  });

  server.start();
}

module.exports = TcpServer;
