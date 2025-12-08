using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace TcpOptimization
{
    class TcpClient
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 8888;
        private const int BUFFER_SIZE = 8192;
        private const int NUM_MESSAGES = 1000;
        private Socket? clientSocket;

        private void SetupSocket(Socket socket)
        {
            try
            {
                // TCP Optimization 1: TCP_NODELAY
                socket.NoDelay = true;

                // TCP Optimization 2: Increase buffer sizes
                socket.SendBufferSize = BUFFER_SIZE * 4;
                socket.ReceiveBufferSize = BUFFER_SIZE * 4;

                // TCP Optimization 3: Connection timeout
                socket.SendTimeout = 5000; // 5 seconds
                socket.ReceiveTimeout = 5000;

                Console.WriteLine("[INFO] Client socket optimizations applied:");
                Console.WriteLine("  - NoDelay (TCP_NODELAY): Enabled");
                Console.WriteLine($"  - SendBufferSize: {socket.SendBufferSize} bytes");
                Console.WriteLine($"  - ReceiveBufferSize: {socket.ReceiveBufferSize} bytes");
                Console.WriteLine($"  - Timeouts: {socket.SendTimeout} ms\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Some optimizations failed: {ex.Message}");
            }
        }

        public void Connect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SetupSocket(clientSocket);

            Console.WriteLine($"Connecting to {SERVER_IP}:{SERVER_PORT}...");

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);
            clientSocket.Connect(serverEndPoint);

            Console.WriteLine("[CONNECTED] Successfully connected to server\n");
        }

        public void RunBenchmark()
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                throw new InvalidOperationException("Not connected to server");
            }

            Console.WriteLine("========================================");
            Console.WriteLine("TCP Client Benchmark (C#)");
            Console.WriteLine("========================================");
            Console.WriteLine($"Sending {NUM_MESSAGES} messages...\n");

            byte[] buffer = new byte[BUFFER_SIZE];
            List<long> latencies = new List<long>(NUM_MESSAGES);
            Stopwatch benchmarkTimer = Stopwatch.StartNew();
            long totalBytes = 0;

            for (int i = 1; i <= NUM_MESSAGES; i++)
            {
                string message = $"Message #{i} from C# client";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                Stopwatch messageTimer = Stopwatch.StartNew();

                // Send message
                int bytesSent = clientSocket.Send(messageBytes);
                if (bytesSent <= 0)
                {
                    Console.WriteLine($"[ERROR] Send failed at message {i}");
                    break;
                }

                // Receive response
                int bytesReceived = clientSocket.Receive(buffer);
                messageTimer.Stop();

                if (bytesReceived <= 0)
                {
                    Console.WriteLine($"[ERROR] Receive failed at message {i}");
                    break;
                }

                totalBytes += bytesSent + bytesReceived;

                // Record latency in microseconds
                long latencyMicros = messageTimer.ElapsedTicks * 1000000 / Stopwatch.Frequency;
                latencies.Add(latencyMicros);

                // Print progress every 100 messages
                if (i % 100 == 0)
                {
                    Console.WriteLine($"[PROGRESS] Sent/Received {i}/{NUM_MESSAGES} messages");
                }
            }

            benchmarkTimer.Stop();

            // Calculate statistics
            if (latencies.Count > 0)
            {
                double avgLatency = latencies.Average();
                long minLatency = latencies.Min();
                long maxLatency = latencies.Max();
                double throughputMBps = (totalBytes / 1024.0 / 1024.0) / (benchmarkTimer.ElapsedMilliseconds / 1000.0);

                Console.WriteLine("\n========================================");
                Console.WriteLine("BENCHMARK RESULTS");
                Console.WriteLine("========================================");
                Console.WriteLine($"Messages Sent:     {latencies.Count}");
                Console.WriteLine($"Total Duration:    {benchmarkTimer.ElapsedMilliseconds} ms");
                Console.WriteLine($"Total Data:        {totalBytes / 1024.0:F2} KB");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Latency (RTT):");
                Console.WriteLine($"  Average:         {avgLatency / 1000.0:F3} ms");
                Console.WriteLine($"  Min:             {minLatency / 1000.0:F3} ms");
                Console.WriteLine($"  Max:             {maxLatency / 1000.0:F3} ms");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Throughput:        {throughputMBps:F2} MB/s");
                Console.WriteLine($"Messages/sec:      {latencies.Count * 1000.0 / benchmarkTimer.ElapsedMilliseconds:F2}");
                Console.WriteLine("========================================");
            }
        }

        public void Disconnect()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
                Console.WriteLine("\n[DISCONNECTED] Connection closed");
            }
        }

        public static void RunClient()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect();

                // Wait to ensure stable connection
                System.Threading.Thread.Sleep(500);

                client.RunBenchmark();

                // Wait before disconnecting
                System.Threading.Thread.Sleep(1000);

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
