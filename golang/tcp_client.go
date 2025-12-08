package main

import (
	"bufio"
	"fmt"
	"log"
	"net"
	"time"
)

const (
	HOST         = "127.0.0.1"
	PORT         = "8888"
	NUM_MESSAGES = 1000
	MESSAGE_SIZE = 100
	BUFFER_SIZE  = 32768
	TIMEOUT      = 5 * time.Second
)

// setSocketOptions applies TCP optimizations to the connection
func setSocketOptions(conn *net.TCPConn) error {
	// TCP_NODELAY - Disable Nagle's algorithm
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

	// Set timeouts
	if err := conn.SetDeadline(time.Now().Add(TIMEOUT)); err != nil {
		return fmt.Errorf("failed to set deadline: %w", err)
	}

	return nil
}

func main() {
	// Connect to server
	fmt.Printf("Connecting to %s:%s...\n", HOST, PORT)
	addr, err := net.ResolveTCPAddr("tcp", HOST+":"+PORT)
	if err != nil {
		log.Fatalf("Failed to resolve address: %v", err)
	}

	conn, err := net.DialTCP("tcp", nil, addr)
	if err != nil {
		log.Fatalf("Failed to connect: %v", err)
	}
	defer conn.Close()

	fmt.Println("[CONNECTED] Successfully connected to server")

	// Apply socket optimizations
	if err := setSocketOptions(conn); err != nil {
		log.Printf("Warning: %v", err)
	}

	fmt.Printf("\n[INFO] Client socket optimizations applied:\n")
	fmt.Printf("  - NoDelay (TCP_NODELAY): Enabled\n")
	fmt.Printf("  - ReadBuffer: %d bytes\n", BUFFER_SIZE)
	fmt.Printf("  - WriteBuffer: %d bytes\n", BUFFER_SIZE)
	fmt.Printf("  - Timeout: %v\n", TIMEOUT)

	reader := bufio.NewReader(conn)
	writer := bufio.NewWriter(conn)

	fmt.Println("\n========================================")
	fmt.Println("TCP Client Benchmark (Go)")
	fmt.Println("========================================")
	fmt.Printf("Sending %d messages...\n\n", NUM_MESSAGES)

	latencies := make([]float64, NUM_MESSAGES)
	totalBytes := 0

	benchmarkStart := time.Now()

	for i := 0; i < NUM_MESSAGES; i++ {
		// Create message with timestamp
		timestamp := time.Now().UnixNano()
		message := fmt.Sprintf("MESSAGE:%d:%d\n", i, timestamp)
		
		sendStart := time.Now()

		// Send message
		if _, err := writer.WriteString(message); err != nil {
			log.Fatalf("Send error: %v", err)
		}
		if err := writer.Flush(); err != nil {
			log.Fatalf("Flush error: %v", err)
		}

		// Receive echo
		response, err := reader.ReadString('\n')
		if err != nil {
			log.Fatalf("Receive error: %v", err)
		}

		// Calculate latency (round-trip time)
		latency := time.Since(sendStart).Seconds() * 1000 // ms
		latencies[i] = latency
		totalBytes += len(response)

		// Progress updates
		if (i+1)%100 == 0 {
			elapsed := time.Since(benchmarkStart).Seconds()
			rate := float64(i+1) / elapsed
			fmt.Printf("[PROGRESS] Sent/Received %d/%d messages\n", i+1, NUM_MESSAGES)
			fmt.Printf("[STATS] Messages: %d, Rate: %.2f msg/sec\n", i+1, rate)
		}
	}

	benchmarkEnd := time.Now()
	totalDuration := benchmarkEnd.Sub(benchmarkStart).Milliseconds()

	// Calculate statistics
	var sumLatency, minLatency, maxLatency float64
	minLatency = latencies[0]
	maxLatency = latencies[0]

	for _, lat := range latencies {
		sumLatency += lat
		if lat < minLatency {
			minLatency = lat
		}
		if lat > maxLatency {
			maxLatency = lat
		}
	}

	avgLatency := sumLatency / float64(NUM_MESSAGES)
	throughputMBps := (float64(totalBytes) / 1024 / 1024) / (float64(totalDuration) / 1000)
	messagesPerSec := float64(NUM_MESSAGES) / (float64(totalDuration) / 1000)

	// Print results
	fmt.Printf("\n========================================\n")
	fmt.Println("BENCHMARK RESULTS")
	fmt.Println("========================================")
	fmt.Printf("Messages Sent:     %d\n", NUM_MESSAGES)
	fmt.Printf("Total Duration:    %d ms\n", totalDuration)
	fmt.Printf("Total Data:        %.2f KB\n", float64(totalBytes)/1024)
	fmt.Println("----------------------------------------")
	fmt.Println("Latency (RTT):")
	fmt.Printf("  Average:         %.3f ms\n", avgLatency)
	fmt.Printf("  Min:             %.3f ms\n", minLatency)
	fmt.Printf("  Max:             %.3f ms\n", maxLatency)
	fmt.Println("----------------------------------------")
	fmt.Printf("Throughput:        %.2f MB/s\n", throughputMBps)
	fmt.Printf("Messages/sec:      %.2f\n", messagesPerSec)
	fmt.Println("========================================")
}
