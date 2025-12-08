const net = require("net");

const SERVER_IP = "127.0.0.1";
const SERVER_PORT = 8888;
const NUM_MESSAGES = 1000;

class TcpClient {
  constructor() {
    this.socket = null;
  }

  setupSocket(socket) {
    try {
      // TCP Optimization 1: TCP_NODELAY
      socket.setNoDelay(true);

      // TCP Optimization 2: Keep-Alive
      socket.setKeepAlive(true, 60000);

      // TCP Optimization 3: Timeout
      socket.setTimeout(5000); // 5 seconds

      console.log("[INFO] Client socket optimizations applied:");
      console.log("  - NoDelay (TCP_NODELAY): Enabled");
      console.log("  - KeepAlive: Enabled");
      console.log("  - Timeout: 5000 ms\n");
    } catch (error) {
      console.error(`[WARNING] Some optimizations failed: ${error.message}`);
    }
  }

  connect() {
    return new Promise((resolve, reject) => {
      console.log(`Connecting to ${SERVER_IP}:${SERVER_PORT}...`);

      this.socket = net.createConnection(
        {
          host: SERVER_IP,
          port: SERVER_PORT,
          // Connection optimizations
          noDelay: true,
          keepAlive: true,
          keepAliveInitialDelay: 60000,
        },
        () => {
          this.setupSocket(this.socket);
          console.log("[CONNECTED] Successfully connected to server\n");
          resolve();
        }
      );

      this.socket.on("error", (error) => {
        reject(error);
      });

      this.socket.on("timeout", () => {
        console.error("[ERROR] Connection timeout");
        this.socket.destroy();
        reject(new Error("Connection timeout"));
      });
    });
  }

  async runBenchmark() {
    if (!this.socket || this.socket.destroyed) {
      throw new Error("Not connected to server");
    }

    console.log("========================================");
    console.log("TCP Client Benchmark (Node.js)");
    console.log("========================================");
    console.log(`Sending ${NUM_MESSAGES} messages...\n`);

    const latencies = [];
    const benchmarkStart = Date.now();
    let totalBytes = 0;
    let messagesCompleted = 0;

    return new Promise((resolve, reject) => {
      let currentMessage = 1;
      let sendTime;

      this.socket.on("data", (data) => {
        const receiveTime = process.hrtime.bigint();
        const latencyNs = receiveTime - sendTime;
        const latencyMicros = Number(latencyNs) / 1000;

        latencies.push(latencyMicros);
        totalBytes += data.length;
        messagesCompleted++;

        // Print progress
        if (messagesCompleted % 100 === 0) {
          console.log(
            `[PROGRESS] Sent/Received ${messagesCompleted}/${NUM_MESSAGES} messages`
          );
        }

        // Send next message or finish
        if (currentMessage < NUM_MESSAGES) {
          currentMessage++;
          sendNextMessage();
        } else {
          // All messages sent and received
          setTimeout(() => {
            this.printResults(
              latencies,
              totalBytes,
              Date.now() - benchmarkStart
            );
            resolve();
          }, 100);
        }
      });

      this.socket.on("error", (error) => {
        console.error(`[ERROR] ${error.message}`);
        reject(error);
      });

      const sendNextMessage = () => {
        const message = `Message #${currentMessage} from Node.js client`;
        sendTime = process.hrtime.bigint();

        const messageBytes = Buffer.from(message, "utf8");
        totalBytes += messageBytes.length;

        this.socket.write(messageBytes);
      };

      // Start sending messages
      sendNextMessage();
    });
  }

  printResults(latencies, totalBytes, totalDuration) {
    if (latencies.length === 0) return;

    // Calculate statistics
    const sumLatency = latencies.reduce((a, b) => a + b, 0);
    const avgLatency = sumLatency / latencies.length;
    const minLatency = Math.min(...latencies);
    const maxLatency = Math.max(...latencies);
    const throughputMBps = totalBytes / 1024 / 1024 / (totalDuration / 1000);

    console.log("\n========================================");
    console.log("BENCHMARK RESULTS");
    console.log("========================================");
    console.log(`Messages Sent:     ${latencies.length}`);
    console.log(`Total Duration:    ${totalDuration} ms`);
    console.log(`Total Data:        ${(totalBytes / 1024).toFixed(2)} KB`);
    console.log("----------------------------------------");
    console.log("Latency (RTT):");
    console.log(`  Average:         ${(avgLatency / 1000).toFixed(3)} ms`);
    console.log(`  Min:             ${(minLatency / 1000).toFixed(3)} ms`);
    console.log(`  Max:             ${(maxLatency / 1000).toFixed(3)} ms`);
    console.log("----------------------------------------");
    console.log(`Throughput:        ${throughputMBps.toFixed(2)} MB/s`);
    console.log(
      `Messages/sec:      ${((latencies.length * 1000) / totalDuration).toFixed(
        2
      )}`
    );
    console.log("========================================");
  }

  disconnect() {
    if (this.socket && !this.socket.destroyed) {
      this.socket.end();
      console.log("\n[DISCONNECTED] Connection closed");
    }
  }
}

// Main execution
async function main() {
  const client = new TcpClient();

  try {
    await client.connect();

    // Wait to ensure stable connection
    await new Promise((resolve) => setTimeout(resolve, 500));

    await client.runBenchmark();

    // Wait before disconnecting
    await new Promise((resolve) => setTimeout(resolve, 1000));

    client.disconnect();

    // Exit after cleanup
    setTimeout(() => process.exit(0), 500);
  } catch (error) {
    console.error(`[ERROR] ${error.message}`);
    process.exit(1);
  }
}

if (require.main === module) {
  main();
}

module.exports = TcpClient;
