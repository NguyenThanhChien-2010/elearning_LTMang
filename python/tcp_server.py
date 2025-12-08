#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import socket
import time
import sys
from datetime import datetime

PORT = 8888
BUFFER_SIZE = 8192
BACKLOG = 10

class TcpServer:
    def __init__(self):
        self.server_socket = None
        self.running = False

    def setup_socket(self, sock):
        """Apply TCP optimizations to the socket"""
        try:
            # TCP Optimization 1: SO_REUSEADDR
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            
            # TCP Optimization 2: TCP_NODELAY (disable Nagle's algorithm)
            sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
            
            # TCP Optimization 3: Increase buffer sizes
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, BUFFER_SIZE * 4)
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, BUFFER_SIZE * 4)
            
            # TCP Optimization 4: Keep-Alive
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)
            
            # Platform-specific Keep-Alive settings
            if hasattr(socket, 'TCP_KEEPIDLE'):
                sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPIDLE, 60)  # 60 seconds
            if hasattr(socket, 'TCP_KEEPINTVL'):
                sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPINTVL, 10)  # 10 seconds
            if hasattr(socket, 'TCP_KEEPCNT'):
                sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPCNT, 5)  # 5 probes
            
            print("[INFO] Socket optimizations applied:")
            print("  - SO_REUSEADDR: Enabled")
            print("  - TCP_NODELAY: Enabled (Nagle disabled)")
            print(f"  - SO_SNDBUF: {sock.getsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF)} bytes")
            print(f"  - SO_RCVBUF: {sock.getsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF)} bytes")
            print("  - SO_KEEPALIVE: Enabled")
            
        except Exception as e:
            print(f"[WARNING] Some optimizations failed: {e}")

    def handle_client(self, client_socket, client_address):
        """Handle a single client connection"""
        try:
            print(f"\n[CONNECTED] Client from {client_address[0]}:{client_address[1]}")
            
            message_count = 0
            start_time = time.time()
            
            while self.running:
                try:
                    # Receive data
                    data = client_socket.recv(BUFFER_SIZE)
                    
                    if not data:
                        print("[DISCONNECTED] Client closed connection")
                        break
                    
                    message_count += 1
                    received_message = data.decode('utf-8')
                    
                    # Create echo response with timestamp
                    timestamp = int(time.time() * 1000)
                    response = f"{received_message} [Server Echo - Msg#{message_count} - Time:{timestamp}]"
                    
                    # Send response
                    client_socket.sendall(response.encode('utf-8'))
                    
                    # Log every 100 messages
                    if message_count % 100 == 0:
                        elapsed = time.time() - start_time
                        messages_per_sec = message_count / elapsed if elapsed > 0 else 0
                        print(f"[STATS] Messages: {message_count}, Rate: {messages_per_sec:.2f} msg/sec")
                        
                except socket.timeout:
                    continue
                except Exception as e:
                    print(f"[ERROR] Error handling message: {e}")
                    break
            
            # Session statistics
            total_duration = time.time() - start_time
            print("\n[SESSION STATS]")
            print(f"  Total Messages: {message_count}")
            print(f"  Total Duration: {total_duration * 1000:.0f} ms")
            if total_duration > 0:
                print(f"  Average Rate: {message_count / total_duration:.2f} msg/sec")
                
        except Exception as e:
            print(f"[ERROR] Client handler error: {e}")
        finally:
            client_socket.close()

    def start(self):
        """Start the TCP server"""
        try:
            # Create socket
            self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.setup_socket(self.server_socket)
            
            # Bind to address
            self.server_socket.bind(('0.0.0.0', PORT))
            
            # Listen for connections
            self.server_socket.listen(BACKLOG)
            
            self.running = True
            
            print("\n" + "=" * 40)
            print("TCP Optimized Server Started (Python)")
            print("=" * 40)
            print(f"Listening on port {PORT}")
            print("Waiting for connections...")
            print("Press Ctrl+C to stop\n")
            
            # Accept connections
            while self.running:
                try:
                    client_socket, client_address = self.server_socket.accept()
                    self.setup_socket(client_socket)
                    self.handle_client(client_socket, client_address)
                except KeyboardInterrupt:
                    print("\n[INFO] Keyboard interrupt received")
                    break
                except Exception as e:
                    if self.running:
                        print(f"[ERROR] Accept error: {e}")
                        
        except Exception as e:
            print(f"[ERROR] Server error: {e}")
            sys.exit(1)
        finally:
            self.stop()

    def stop(self):
        """Stop the server"""
        self.running = False
        if self.server_socket:
            self.server_socket.close()
            print("\n[STOPPED] Server stopped")

if __name__ == "__main__":
    server = TcpServer()
    try:
        server.start()
    except KeyboardInterrupt:
        print("\n[INFO] Shutting down...")
        server.stop()
