using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace TcpOptimization
{
    class TcpServer
    {
        private const int PORT = 8888;
        private const int BUFFER_SIZE = 8192;
        private const int BACKLOG = 10;
        private TcpListener? listener;
        private bool running = false;

        private void SetupSocket(Socket socket)
        {
            try
            {
                // TCP Optimization 1: TCP_NODELAY (disable Nagle's algorithm)
                socket.NoDelay = true;

                // TCP Optimization 2: Increase buffer sizes
                socket.SendBufferSize = BUFFER_SIZE * 4;
                socket.ReceiveBufferSize = BUFFER_SIZE * 4;

                // TCP Optimization 3: Set socket options for performance
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // TCP Optimization 4: Keep-Alive
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                // Windows-specific Keep-Alive settings
                byte[] keepAliveValues = new byte[12];
                BitConverter.GetBytes((uint)1).CopyTo(keepAliveValues, 0);  // on/off
                BitConverter.GetBytes((uint)60000).CopyTo(keepAliveValues, 4);  // keep-alive time (60 sec)
                BitConverter.GetBytes((uint)10000).CopyTo(keepAliveValues, 8);  // keep-alive interval (10 sec)
                socket.IOControl(IOControlCode.KeepAliveValues, keepAliveValues, null);

                Console.WriteLine("[INFO] Socket optimizations applied:");
                Console.WriteLine("  - NoDelay (TCP_NODELAY): Enabled");
                Console.WriteLine($"  - SendBufferSize: {socket.SendBufferSize} bytes");
                Console.WriteLine($"  - ReceiveBufferSize: {socket.ReceiveBufferSize} bytes");
                Console.WriteLine("  - KeepAlive: Enabled");
                Console.WriteLine("  - ReuseAddress: Enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Some optimizations failed: {ex.Message}");
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                IPEndPoint? remoteEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"\n[CONNECTED] Client from {remoteEndPoint?.Address}:{remoteEndPoint?.Port}");

                byte[] buffer = new byte[BUFFER_SIZE];
                int messageCount = 0;
                Stopwatch sessionTimer = Stopwatch.StartNew();

                while (running && clientSocket.Connected)
                {
                    int bytesReceived = clientSocket.Receive(buffer);
                    
                    if (bytesReceived <= 0)
                    {
                        Console.WriteLine("[DISCONNECTED] Client closed connection");
                        break;
                    }

                    messageCount++;
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                    // Create echo response with timestamp
                    long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    string response = $"{receivedMessage} [Server Echo - Msg#{messageCount} - Time:{timestamp}]";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);

                    int bytesSent = clientSocket.Send(responseBytes);

                    if (bytesSent <= 0)
                    {
                        Console.WriteLine("[ERROR] Send failed");
                        break;
                    }

                    // Log every 100 messages
                    if (messageCount % 100 == 0)
                    {
                        double messagesPerSec = messageCount * 1000.0 / sessionTimer.ElapsedMilliseconds;
                        Console.WriteLine($"[STATS] Messages: {messageCount}, Rate: {messagesPerSec:F2} msg/sec");
                    }
                }

                sessionTimer.Stop();

                // Session statistics
                Console.WriteLine("\n[SESSION STATS]");
                Console.WriteLine($"  Total Messages: {messageCount}");
                Console.WriteLine($"  Total Duration: {sessionTimer.ElapsedMilliseconds} ms");
                if (sessionTimer.ElapsedMilliseconds > 0)
                {
                    double avgRate = messageCount * 1000.0 / sessionTimer.ElapsedMilliseconds;
                    Console.WriteLine($"  Average Rate: {avgRate:F2} msg/sec");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[ERROR] Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
            }
        }

        public void Start()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start(BACKLOG);

                // Apply optimizations to the server socket
                SetupSocket(listener.Server);

                running = true;

                Console.WriteLine("\n========================================");
                Console.WriteLine("TCP Optimized Server Started (C#)");
                Console.WriteLine("========================================");
                Console.WriteLine($"Listening on port {PORT}");
                Console.WriteLine("Waiting for connections...");
                Console.WriteLine("Press Ctrl+C to stop\n");

                while (running)
                {
                    Socket clientSocket = listener.AcceptSocket();
                    SetupSocket(clientSocket);
                    HandleClient(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Server error: {ex.Message}");
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            running = false;
            listener?.Stop();
            Console.WriteLine("\n[STOPPED] Server stopped");
        }

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "client")
            {
                TcpClient.RunClient();
            }
            else
            {
                TcpServer server = new TcpServer();
                
                // Handle Ctrl+C gracefully
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    server.Stop();
                };

                server.Start();
            }
        }
    }
}
