import java.io.*;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.*;

public class TcpClient {
    private static final String SERVER_IP = "127.0.0.1";
    private static final int SERVER_PORT = 8888;
    private static final int BUFFER_SIZE = 8192;
    private static final int NUM_MESSAGES = 1000;
    private Socket clientSocket;

    private void setupSocket(Socket socket) throws SocketException {
        try {
            // TCP Optimization 1: TCP_NODELAY
            socket.setTcpNoDelay(true);

            // TCP Optimization 2: Increase buffer sizes
            socket.setSendBufferSize(BUFFER_SIZE * 4);
            socket.setReceiveBufferSize(BUFFER_SIZE * 4);

            // TCP Optimization 3: Connection timeout
            socket.setSoTimeout(5000); // 5 seconds

            System.out.println("[INFO] Client socket optimizations applied:");
            System.out.println("  - TcpNoDelay: " + socket.getTcpNoDelay());
            System.out.println("  - SendBufferSize: " + socket.getSendBufferSize() + " bytes");
            System.out.println("  - ReceiveBufferSize: " + socket.getReceiveBufferSize() + " bytes");
            System.out.println("  - SoTimeout: " + socket.getSoTimeout() + " ms\n");

        } catch (SocketException e) {
            System.err.println("[WARNING] Some optimizations failed: " + e.getMessage());
        }
    }

    public void connect() throws IOException {
        System.out.println("Connecting to " + SERVER_IP + ":" + SERVER_PORT + "...");
        
        clientSocket = new Socket();
        clientSocket.connect(new InetSocketAddress(SERVER_IP, SERVER_PORT), 5000);
        setupSocket(clientSocket);
        
        System.out.println("[CONNECTED] Successfully connected to server\n");
    }

    public void runBenchmark() throws IOException {
        if (clientSocket == null || !clientSocket.isConnected()) {
            throw new IllegalStateException("Not connected to server");
        }

        System.out.println("========================================");
        System.out.println("TCP Client Benchmark (Java)");
        System.out.println("========================================");
        System.out.println("Sending " + NUM_MESSAGES + " messages...\n");

        InputStream input = clientSocket.getInputStream();
        OutputStream output = clientSocket.getOutputStream();
        byte[] buffer = new byte[BUFFER_SIZE];

        List<Long> latencies = new ArrayList<>(NUM_MESSAGES);
        long benchmarkStart = System.currentTimeMillis();
        long totalBytes = 0;

        for (int i = 1; i <= NUM_MESSAGES; i++) {
            String message = "Message #" + i + " from Java client";
            byte[] messageBytes = message.getBytes(StandardCharsets.UTF_8);

            long sendTime = System.nanoTime();

            // Send message
            output.write(messageBytes);
            output.flush();

            // Receive response
            int bytesRead = input.read(buffer);

            long receiveTime = System.nanoTime();

            if (bytesRead == -1) {
                System.err.println("[ERROR] Connection closed at message " + i);
                break;
            }

            totalBytes += messageBytes.length + bytesRead;

            // Calculate latency in microseconds
            long latencyMicros = (receiveTime - sendTime) / 1000;
            latencies.add(latencyMicros);

            // Print progress every 100 messages
            if (i % 100 == 0) {
                System.out.println("[PROGRESS] Sent/Received " + i + "/" + NUM_MESSAGES + " messages");
            }
        }

        long benchmarkEnd = System.currentTimeMillis();
        long totalDuration = benchmarkEnd - benchmarkStart;

        // Calculate statistics
        if (!latencies.isEmpty()) {
            long sumLatency = 0;
            long minLatency = latencies.get(0);
            long maxLatency = latencies.get(0);

            for (long lat : latencies) {
                sumLatency += lat;
                if (lat < minLatency) minLatency = lat;
                if (lat > maxLatency) maxLatency = lat;
            }

            double avgLatency = sumLatency / (double) latencies.size();
            double throughputMBps = (totalBytes / 1024.0 / 1024.0) / (totalDuration / 1000.0);

            System.out.println("\n========================================");
            System.out.println("BENCHMARK RESULTS");
            System.out.println("========================================");
            System.out.println("Messages Sent:     " + latencies.size());
            System.out.println("Total Duration:    " + totalDuration + " ms");
            System.out.printf("Total Data:        %.2f KB\n", totalBytes / 1024.0);
            System.out.println("----------------------------------------");
            System.out.println("Latency (RTT):");
            System.out.printf("  Average:         %.3f ms\n", avgLatency / 1000.0);
            System.out.printf("  Min:             %.3f ms\n", minLatency / 1000.0);
            System.out.printf("  Max:             %.3f ms\n", maxLatency / 1000.0);
            System.out.println("----------------------------------------");
            System.out.printf("Throughput:        %.2f MB/s\n", throughputMBps);
            System.out.printf("Messages/sec:      %.2f\n", latencies.size() * 1000.0 / totalDuration);
            System.out.println("========================================");
        }
    }

    public void disconnect() {
        if (clientSocket != null && !clientSocket.isClosed()) {
            try {
                clientSocket.close();
                System.out.println("\n[DISCONNECTED] Connection closed");
            } catch (IOException e) {
                System.err.println("[ERROR] Error closing connection: " + e.getMessage());
            }
        }
    }

    public static void main(String[] args) {
        TcpClient client = new TcpClient();
        
        try {
            client.connect();
            
            // Wait to ensure stable connection
            Thread.sleep(500);
            
            client.runBenchmark();
            
            // Wait before disconnecting
            Thread.sleep(1000);
            
            client.disconnect();
            
        } catch (IOException e) {
            System.err.println("[ERROR] " + e.getMessage());
            System.exit(1);
        } catch (InterruptedException e) {
            System.err.println("[ERROR] Interrupted: " + e.getMessage());
            Thread.currentThread().interrupt();
            System.exit(1);
        }
    }
}
