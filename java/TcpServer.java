import java.io.*;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.*;

public class TcpServer {
    private static final int PORT = 8888;
    private static final int BUFFER_SIZE = 8192;
    private static final int BACKLOG = 10;
    private ServerSocket serverSocket;
    private volatile boolean running = false;

    private void setupSocket(Socket socket) throws SocketException {
        try {
            // TCP Optimization 1: TCP_NODELAY (disable Nagle's algorithm)
            socket.setTcpNoDelay(true);

            // TCP Optimization 2: Increase buffer sizes
            socket.setSendBufferSize(BUFFER_SIZE * 4);
            socket.setReceiveBufferSize(BUFFER_SIZE * 4);

            // TCP Optimization 3: Keep-Alive
            socket.setKeepAlive(true);

            // TCP Optimization 4: Reuse address
            socket.setReuseAddress(true);

            System.out.println("[INFO] Socket optimizations applied:");
            System.out.println("  - TcpNoDelay: " + socket.getTcpNoDelay());
            System.out.println("  - SendBufferSize: " + socket.getSendBufferSize() + " bytes");
            System.out.println("  - ReceiveBufferSize: " + socket.getReceiveBufferSize() + " bytes");
            System.out.println("  - KeepAlive: " + socket.getKeepAlive());
            System.out.println("  - ReuseAddress: " + socket.getReuseAddress());

        } catch (SocketException e) {
            System.err.println("[WARNING] Some optimizations failed: " + e.getMessage());
        }
    }

    private void handleClient(Socket clientSocket) {
        try {
            InetAddress clientAddr = clientSocket.getInetAddress();
            int clientPort = clientSocket.getPort();
            System.out.println("\n[CONNECTED] Client from " + clientAddr.getHostAddress() + ":" + clientPort);

            InputStream input = clientSocket.getInputStream();
            OutputStream output = clientSocket.getOutputStream();
            byte[] buffer = new byte[BUFFER_SIZE];

            int messageCount = 0;
            long startTime = System.currentTimeMillis();

            while (running && !clientSocket.isClosed()) {
                int bytesRead = input.read(buffer);

                if (bytesRead == -1) {
                    System.out.println("[DISCONNECTED] Client closed connection");
                    break;
                }

                messageCount++;
                String receivedMessage = new String(buffer, 0, bytesRead, StandardCharsets.UTF_8);

                // Create echo response with timestamp
                long timestamp = System.currentTimeMillis();
                String response = receivedMessage + " [Server Echo - Msg#" + messageCount + 
                                " - Time:" + timestamp + "]";
                byte[] responseBytes = response.getBytes(StandardCharsets.UTF_8);

                output.write(responseBytes);
                output.flush();

                // Log every 100 messages
                if (messageCount % 100 == 0) {
                    long currentTime = System.currentTimeMillis();
                    long duration = currentTime - startTime;
                    double messagesPerSec = (messageCount * 1000.0) / duration;
                    System.out.printf("[STATS] Messages: %d, Rate: %.2f msg/sec\n", 
                                    messageCount, messagesPerSec);
                }
            }

            long endTime = System.currentTimeMillis();
            long totalDuration = endTime - startTime;

            System.out.println("\n[SESSION STATS]");
            System.out.println("  Total Messages: " + messageCount);
            System.out.println("  Total Duration: " + totalDuration + " ms");
            if (totalDuration > 0) {
                double avgRate = (messageCount * 1000.0) / totalDuration;
                System.out.printf("  Average Rate: %.2f msg/sec\n", avgRate);
            }

        } catch (IOException e) {
            System.err.println("[ERROR] Client handler error: " + e.getMessage());
        } finally {
            try {
                clientSocket.close();
            } catch (IOException e) {
                System.err.println("[ERROR] Error closing client socket: " + e.getMessage());
            }
        }
    }

    public void start() {
        try {
            serverSocket = new ServerSocket(PORT, BACKLOG);
            serverSocket.setReuseAddress(true);

            running = true;

            System.out.println("\n========================================");
            System.out.println("TCP Optimized Server Started (Java)");
            System.out.println("========================================");
            System.out.println("Listening on port " + PORT);
            System.out.println("Waiting for connections...");
            System.out.println("Press Ctrl+C to stop\n");

            while (running) {
                try {
                    Socket clientSocket = serverSocket.accept();
                    setupSocket(clientSocket);
                    handleClient(clientSocket);
                } catch (SocketException e) {
                    if (running) {
                        System.err.println("[ERROR] Accept error: " + e.getMessage());
                    }
                } catch (IOException e) {
                    if (running) {
                        System.err.println("[ERROR] I/O error: " + e.getMessage());
                    }
                }
            }

        } catch (IOException e) {
            System.err.println("[ERROR] Server error: " + e.getMessage());
            System.exit(1);
        } finally {
            stop();
        }
    }

    public void stop() {
        running = false;
        if (serverSocket != null && !serverSocket.isClosed()) {
            try {
                serverSocket.close();
                System.out.println("\n[STOPPED] Server stopped");
            } catch (IOException e) {
                System.err.println("[ERROR] Error closing server socket: " + e.getMessage());
            }
        }
    }

    public static void main(String[] args) {
        TcpServer server = new TcpServer();

        // Add shutdown hook for graceful shutdown
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            System.out.println("\n[INFO] Shutdown signal received");
            server.stop();
        }));

        server.start();
    }
}
