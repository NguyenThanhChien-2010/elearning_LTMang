package main

import (
	"bufio"
	"fmt"
	"log"
	"net"
	"os"
	"os/signal"
	"syscall"
	"time"
)

const (
	PORT        = "8888"
	BUFFER_SIZE = 32768
)

// setSocketOptions applies TCP optimizations to the connection
func setSocketOptions(conn *net.TCPConn) error {
	// TCP_NODELAY - Disable Nagle's algorithm for low latency
	if err := conn.SetNoDelay(true); err != nil {
		return fmt.Errorf("failed to set TCP_NODELAY: %w", err)
	}

	// Set buffer sizes
	if err := conn.SetReadBuffer(BUFFER_SIZE); err != nil {
		return fmt.Errorf("failed to set read buffer: %w", err)
	}
	if err := conn.SetWriteBuffer(BUFFER_SIZE); err != nil {
		return fmt.Errorf("failed to set write buffer: %w", err)
	}

	// KeepAlive
	if err := conn.SetKeepAlive(true); err != nil {
		return fmt.Errorf("failed to set keep-alive: %w", err)
	}
	if err := conn.SetKeepAlivePeriod(30 * time.Second); err != nil {
		return fmt.Errorf("failed to set keep-alive period: %w", err)
	}

	return nil
}

func handleClient(conn *net.TCPConn) {
	defer conn.Close()

	clientAddr := conn.RemoteAddr().String()
	fmt.Printf("\n[INFO] Socket optimizations applied:\n")
	fmt.Printf("  - NoDelay (TCP_NODELAY): Enabled\n")
	fmt.Printf("  - ReadBuffer: %d bytes\n", BUFFER_SIZE)
	fmt.Printf("  - WriteBuffer: %d bytes\n", BUFFER_SIZE)
	fmt.Printf("  - KeepAlive: Enabled (30s period)\n")
	fmt.Printf("\n[CONNECTED] Client from %s\n", clientAddr)

	reader := bufio.NewReader(conn)
	writer := bufio.NewWriter(conn)
	
	messageCount := 0
	startTime := time.Now()

	for {
		// Read message (format: "MESSAGE:<id>:<timestamp>")
		message, err := reader.ReadString('\n')
		if err != nil {
			break
		}

		messageCount++

		// Echo back
		if _, err := writer.WriteString(message); err != nil {
			break
		}
		if err := writer.Flush(); err != nil {
			break
		}

		// Progress updates
		if messageCount%100 == 0 {
			elapsed := time.Since(startTime).Milliseconds()
			rate := float64(messageCount) / (float64(elapsed) / 1000.0)
			fmt.Printf("[STATS] Messages: %d, Rate: %.2f msg/sec\n", messageCount, rate)
		}
	}

	duration := time.Since(startTime).Milliseconds()
	avgRate := float64(messageCount) / (float64(duration) / 1000.0)

	fmt.Printf("\n[SESSION STATS]\n")
	fmt.Printf("  Total Messages: %d\n", messageCount)
	fmt.Printf("  Total Duration: %d ms\n", duration)
	fmt.Printf("  Average Rate: %.2f msg/sec\n", avgRate)
	fmt.Printf("\n[DISCONNECTED] Connection closed\n")
}

func main() {
	// Parse address
	addr, err := net.ResolveTCPAddr("tcp", "0.0.0.0:"+PORT)
	if err != nil {
		log.Fatalf("Failed to resolve address: %v", err)
	}

	// Create listener
	listener, err := net.ListenTCP("tcp", addr)
	if err != nil {
		log.Fatalf("Failed to create listener: %v", err)
	}
	defer listener.Close()

	fmt.Println("\n========================================")
	fmt.Println("TCP Optimized Server Started (Go)")
	fmt.Println("========================================")
	fmt.Printf("Listening on port %s\n", PORT)
	fmt.Println("Waiting for connections...")
	fmt.Println("Press Ctrl+C to stop\n")

	// Handle graceful shutdown
	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, os.Interrupt, syscall.SIGTERM)
	go func() {
		<-sigChan
		fmt.Println("\n\n[SHUTDOWN] Server stopping...")
		listener.Close()
		os.Exit(0)
	}()

	// Accept connections
	for {
		conn, err := listener.AcceptTCP()
		if err != nil {
			continue
		}

		// Apply socket optimizations
		if err := setSocketOptions(conn); err != nil {
			log.Printf("Warning: %v", err)
		}

		// Handle client in goroutine
		go handleClient(conn)
	}
}
