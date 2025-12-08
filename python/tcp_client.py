#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import socket
import time
import sys
import statistics

SERVER_IP = "127.0.0.1"
SERVER_PORT = 8888
BUFFER_SIZE = 8192
NUM_MESSAGES = 1000

class TcpClient:
    def __init__(self):
        self.client_socket = None

    def setup_socket(self, sock):
        """Apply TCP optimizations to the socket"""
        try:
            # TCP Optimization 1: TCP_NODELAY
            sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
            
            # TCP Optimization 2: Increase buffer sizes
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, BUFFER_SIZE * 4)
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, BUFFER_SIZE * 4)
            
            # TCP Optimization 3: Connection timeout
            sock.settimeout(5.0)  # 5 seconds
            
            print("[INFO] Client socket optimizations applied:")
            print("  - TCP_NODELAY: Enabled")
            print(f"  - SO_SNDBUF: {sock.getsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF)} bytes")
            print(f"  - SO_RCVBUF: {sock.getsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF)} bytes")
            print(f"  - Timeout: {sock.gettimeout()} seconds\n")
            
        except Exception as e:
            print(f"[WARNING] Some optimizations failed: {e}")

    def connect(self):
        """Connect to the server"""
        try:
            self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.setup_socket(self.client_socket)
            
            print(f"Connecting to {SERVER_IP}:{SERVER_PORT}...")
            self.client_socket.connect((SERVER_IP, SERVER_PORT))
            print("[CONNECTED] Successfully connected to server\n")
            
        except Exception as e:
            print(f"[ERROR] Connection failed: {e}")
            sys.exit(1)

    def run_benchmark(self):
        """Run benchmark test"""
        if not self.client_socket:
            raise RuntimeError("Not connected to server")
        
        print("=" * 40)
        print("TCP Client Benchmark (Python)")
        print("=" * 40)
        print(f"Sending {NUM_MESSAGES} messages...\n")
        
        latencies = []
        benchmark_start = time.time()
        total_bytes = 0
        
        for i in range(1, NUM_MESSAGES + 1):
            message = f"Message #{i} from Python client"
            message_bytes = message.encode('utf-8')
            
            # Measure round-trip time
            send_time = time.perf_counter()
            
            try:
                # Send message
                self.client_socket.sendall(message_bytes)
                
                # Receive response
                response = self.client_socket.recv(BUFFER_SIZE)
                
                receive_time = time.perf_counter()
                
                if not response:
                    print(f"[ERROR] No response at message {i}")
                    break
                
                total_bytes += len(message_bytes) + len(response)
                
                # Calculate latency in microseconds
                latency_us = (receive_time - send_time) * 1_000_000
                latencies.append(latency_us)
                
                # Print progress every 100 messages
                if i % 100 == 0:
                    print(f"[PROGRESS] Sent/Received {i}/{NUM_MESSAGES} messages")
                    
            except socket.timeout:
                print(f"[ERROR] Timeout at message {i}")
                break
            except Exception as e:
                print(f"[ERROR] Error at message {i}: {e}")
                break
        
        benchmark_end = time.time()
        total_duration_ms = (benchmark_end - benchmark_start) * 1000
        
        # Calculate statistics
        if latencies:
            avg_latency = statistics.mean(latencies)
            min_latency = min(latencies)
            max_latency = max(latencies)
            throughput_mbps = (total_bytes / 1024 / 1024) / ((benchmark_end - benchmark_start))
            
            print("\n" + "=" * 40)
            print("BENCHMARK RESULTS")
            print("=" * 40)
            print(f"Messages Sent:     {len(latencies)}")
            print(f"Total Duration:    {total_duration_ms:.0f} ms")
            print(f"Total Data:        {total_bytes / 1024:.2f} KB")
            print("-" * 40)
            print("Latency (RTT):")
            print(f"  Average:         {avg_latency / 1000:.3f} ms")
            print(f"  Min:             {min_latency / 1000:.3f} ms")
            print(f"  Max:             {max_latency / 1000:.3f} ms")
            print("-" * 40)
            print(f"Throughput:        {throughput_mbps:.2f} MB/s")
            print(f"Messages/sec:      {len(latencies) * 1000 / total_duration_ms:.2f}")
            print("=" * 40)

    def disconnect(self):
        """Disconnect from the server"""
        if self.client_socket:
            self.client_socket.close()
            self.client_socket = None
            print("\n[DISCONNECTED] Connection closed")

if __name__ == "__main__":
    try:
        client = TcpClient()
        client.connect()
        
        # Wait to ensure stable connection
        time.sleep(0.5)
        
        client.run_benchmark()
        
        # Wait before disconnecting
        time.sleep(1)
        
        client.disconnect()
        
    except KeyboardInterrupt:
        print("\n[INFO] Interrupted by user")
    except Exception as e:
        print(f"[ERROR] {e}")
        sys.exit(1)
