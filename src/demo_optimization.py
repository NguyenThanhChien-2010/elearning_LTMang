import threading
import time
from optimized_udp_server import OptimizedUDPServer
from optimized_udp_client import OptimizedUDPClient

def run_demo():
    """Chạy demo đầy đủ các kỹ thuật tối ưu hóa"""
    
    print("UDP PROTOCOL OPTIMIZATION DEMO")
    print("=" * 60)
    print("Môn: Lập trình Mạng - Elearning Project")
    print("=" * 60)
    
    # Khởi động server trong thread riêng
    server = OptimizedUDPServer(port=8888)
    server_thread = threading.Thread(target=server.start, daemon=True)
    server_thread.start()
    
    # Chờ server khởi động
    time.sleep(2)
    
    print("\nBAT DAU DEMO CLIENT")
    print("-" * 40)
    
    # Khởi động client
    client = OptimizedUDPClient(server_port=8888)
    client.start_demo()
    
    print("\nKET THUC DEMO!")
    print("Các kỹ thuật đã được minh họa:")
    print("  Packet Bundling")
    print("  Selective Retransmission") 
    print("  ACK-based Reliability")
    print("  Loss Detection & Handling")
    print("  Sequence Numbering")
    print("  Duplicate Prevention")

if __name__ == "__main__":
    run_demo()
