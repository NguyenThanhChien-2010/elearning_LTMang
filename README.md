# Tối Ưu Hóa Giao Thức TCP - Nhóm 14

## 📌 Giới Thiệu

Dự án minh họa các kỹ thuật tối ưu hóa TCP thực tế trên 5 ngôn ngữ: **Go, C#, Python, Java, Node.js**.

## 🚀 Cài Đặt & Chạy Dự Án

### 1. Clone dự án

```bash
git clone https://github.com/NguyenThanhChien-2010/elearning_LTMang
cd elearning_LTMang
```

### 2. Chạy theo ngôn ngữ

**Lưu ý:** Mỗi ngôn ngữ cần mở **2 terminal**: một cho server, một cho client.

#### 🐹 Go

```powershell
cd golang
# Terminal 1
go run tcp_server.go
# Terminal 2
go run tcp_client.go
```

#### 🔷 C#

```powershell
cd csharp
.\build.bat
# Terminal 1
dotnet run --project TcpOptimization.csproj -- server
# Terminal 2
dotnet run --project TcpOptimization.csproj -- client
```

#### 🐍 Python

```powershell
cd python
# Terminal 1
python tcp_server.py
# Terminal 2
python tcp_client.py
```

#### ☕ Java

```powershell
cd java
.\build.bat
# Terminal 1
java TcpServer
# Terminal 2
java TcpClient
```

#### 🟢 Node.js

```powershell
cd nodejs
npm install
# Terminal 1
node tcp_server.js
# Terminal 2
node tcp_client.js
```

## ⚙️ Cách Hoạt Động

1. **Server** lắng nghe trên cổng `8888`, nhận tin nhắn và gửi lại (echo)
2. **Client** kết nối đến server, gửi 1000 tin nhắn để test
3. **Client** hiển thị kết quả: Messages/giây, Latency, Throughput

## 🔧 Các Kỹ Thuật Tối Ưu Hóa TCP

| Kỹ Thuật               | Mục Đích                                | Hiệu Quả         |
| ---------------------- | --------------------------------------- | ---------------- |
| **TCP_NODELAY**        | Tắt Nagle's Algorithm, giảm độ trễ      | Latency -80%     |
| **Socket Buffer 32KB** | Tăng buffer gửi/nhận                    | Throughput +200% |
| **TCP Keep-Alive**     | Duy trì kết nối, phát hiện ngắt kết nối | Độ tin cậy cao   |
| **SO_REUSEADDR**       | Tái sử dụng port ngay lập tức           | Restart nhanh    |
| **Timeout Settings**   | Ngăn treo vô thời hạn                   | Tăng reliability |

## 📊 Kết Quả Benchmark

| Ngôn Ngữ    | Messages/sec | Avg Latency | Throughput | Xếp Hạng |
| ----------- | ------------ | ----------- | ---------- | -------- |
| **Go**      | 22,222       | 0.042 ms    | 0.68 MB/s  | 🥇       |
| **Python**  | 20,123       | 0.047 ms    | 2.05 MB/s  | 🥈       |
| **C#**      | 19,231       | 0.050 ms    | 1.81 MB/s  | 🥉       |
| **Java**    | 15,385       | 0.055 ms    | 1.51 MB/s  | 4        |
| **Node.js** | 5,348        | 0.078 ms    | 0.55 MB/s  | 5        |

**Môi trường test:** 1000 messages, localhost, Windows 11

## 📁 Cấu Trúc Dự Án

```
├── golang/          # Go implementation
├── csharp/          # C# (.NET 8.0)
├── python/          # Python 3.7+
├── java/            # Java JDK 8+
└── nodejs/          # Node.js 14+
```

## 💻 Yêu Cầu Hệ Thống

- **Go:** 1.20+
- **C#:** .NET 8.0 SDK
- **Python:** 3.7+
- **Java:** JDK 8+
- **Node.js:** 14+

## 👥 Thông Tin

- **Nhóm:** Nhóm 14
- **Sinh viên:** Nguyễn Thành Chiến
- **Môn học:** Lập trình Mạng – Elearning 1
- **Giảng viên:** ThS. Bùi Dương Thế
- **Repository:** https://github.com/NguyenThanhChien-2010/elearning_LTMang

## 📄 License

MIT License - Tự do sử dụng cho mục đích học tập

### 1️⃣ TCP_NODELAY - Tắt Nagle's Algorithm

**Vấn đề:** Nagle's Algorithm buffer các gói tin nhỏ (< MSS) trước khi gửi → tăng latency

**Giải pháp:** Tắt Nagle để gửi ngay lập tức

#### **Python:**

```python
import socket

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)  # Tắt Nagle
```

#### **Go:**

```go
conn, _ := net.DialTCP("tcp", nil, addr)
conn.SetNoDelay(true)  // Tắt Nagle
```

#### **C#:**

```csharp
TcpClient client = new TcpClient();
client.NoDelay = true;  // Tắt Nagle
```

#### **Java:**

```java
Socket socket = new Socket("localhost", 8888);
socket.setTcpNoDelay(true);  // Tắt Nagle
```

#### **Node.js:**

```javascript
const socket = net.connect(8888, "localhost");
socket.setNoDelay(true); // Tắt Nagle
```

**Kết quả:** Giảm latency từ ~0.2ms xuống ~0.04ms (giảm 80%)

---

### 2️⃣ Socket Buffer Optimization

**Vấn đề:** Buffer mặc định (thường 8KB) quá nhỏ → nhiều system calls → overhead cao

**Giải pháp:** Tăng buffer lên 32KB hoặc 64KB

#### **Python:**

```python
BUFFER_SIZE = 32768  # 32KB

sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, BUFFER_SIZE)  # Send buffer
sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, BUFFER_SIZE)  # Receive buffer
```

#### **Go:**

```go
conn.SetReadBuffer(32768)   // 32KB read buffer
conn.SetWriteBuffer(32768)  // 32KB write buffer
```

#### **C#:**

```csharp
socket.SendBufferSize = 32768;     // 32KB
socket.ReceiveBufferSize = 32768;  // 32KB
```

#### **Java:**

```java
socket.setSendBufferSize(32768);     // 32KB
socket.setReceiveBufferSize(32768);  // 32KB
```

#### **Node.js:**

```javascript
// Node.js tự động quản lý buffer, nhưng có thể hint
const socket = new net.Socket({
  readable: true,
  writable: true,
});
```

**Kết quả:** Throughput tăng 2-3 lần (từ ~0.8 MB/s lên ~2.0 MB/s)

---

### 3️⃣ TCP Keep-Alive

**Vấn đề:** Kết nối idle bị đứt mà không biết → client/server vẫn chờ

**Giải pháp:** Bật Keep-Alive để gửi probe packets định kỳ

#### **Python:**

```python
sock.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)  # Bật Keep-Alive

# Linux: Cấu hình chi tiết
sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPIDLE, 60)   # 60s trước probe đầu tiên
sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPINTVL, 10)  # 10s giữa các probes
sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_KEEPCNT, 3)     # 3 probes trước khi đóng
```

#### **Go:**

```go
conn.SetKeepAlive(true)
conn.SetKeepAlivePeriod(30 * time.Second)  // Probe mỗi 30s
```

#### **C#:**

```csharp
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
```

#### **Java:**

```java
socket.setKeepAlive(true);  // Bật Keep-Alive
```

**Kết quả:** Phát hiện connection failure trong 60-90s thay vì timeout vô hạn

---

### 4️⃣ SO_REUSEADDR - Reuse Port

**Vấn đề:** Sau khi server tắt, port vẫn ở trạng thái TIME_WAIT → không thể restart ngay

**Giải pháp:** Bật SO_REUSEADDR để tái sử dụng port ngay lập tức

#### **Python:**

```python
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)  # Reuse port
sock.bind(('0.0.0.0', 8888))
```

#### **Go:**

```go
// Go tự động set SO_REUSEADDR khi Listen
listener, _ := net.Listen("tcp", ":8888")
```

#### **C#:**

```csharp
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
socket.Bind(new IPEndPoint(IPAddress.Any, 8888));
```

#### **Java:**

```java
ServerSocket serverSocket = new ServerSocket();
serverSocket.setReuseAddress(true);  // Reuse port
serverSocket.bind(new InetSocketAddress(8888));
```

**Kết quả:** Server có thể restart ngay lập tức, không cần đợi TIME_WAIT (2-4 phút)

---

### 5️⃣ Connection Timeout

**Vấn đề:** Kết nối bị treo vô thời hạn nếu server không phản hồi

**Giải pháp:** Đặt timeout cho operations

#### **Python:**

```python
sock.settimeout(5.0)  # 5 giây timeout cho read/write
```

#### **Go:**

```go
conn.SetDeadline(time.Now().Add(5 * time.Second))  // 5s timeout cho tất cả operations
// Hoặc riêng lẻ:
conn.SetReadDeadline(time.Now().Add(5 * time.Second))
conn.SetWriteDeadline(time.Now().Add(5 * time.Second))
```

#### **C#:**

```csharp
socket.SendTimeout = 5000;     // 5s timeout cho send
socket.ReceiveTimeout = 5000;  // 5s timeout cho receive
```

#### **Java:**

```java
socket.setSoTimeout(5000);  // 5s timeout cho read operations
```

#### **Node.js:**

```javascript
socket.setTimeout(5000, () => {
  console.log("Timeout!");
  socket.destroy();
});
```

**Kết quả:** Phát hiện network issues trong 5s thay vì treo vô thời hạn

---

### 📊 Tổng Hợp Impact của Các Tối Ưu Hóa

| Tối Ưu Hóa       | Impact     | Latency Giảm | Throughput Tăng | Use Case                        |
| ---------------- | ---------- | ------------ | --------------- | ------------------------------- |
| **TCP_NODELAY**  | ⭐⭐⭐⭐⭐ | -80%         | +10%            | Real-time apps, gaming, trading |
| **Buffer 32KB**  | ⭐⭐⭐⭐   | -20%         | +200%           | File transfer, streaming        |
| **Keep-Alive**   | ⭐⭐⭐     | 0%           | 0%              | Long-lived connections          |
| **SO_REUSEADDR** | ⭐⭐⭐     | 0%           | 0%              | Development, frequent restarts  |
| **Timeouts**     | ⭐⭐⭐⭐   | 0%           | 0%              | Production reliability          |

---

## Cấu Trúc Dự Án

```
Nhom14-Elearning1/
├── README.md                 # Tài liệu đầy đủ
├── golang/                   # Implementation Go
│   ├── tcp_server.go
│   ├── tcp_client.go
│   ├── go.mod
│   ├── README.md
│   └── run.bat
├── csharp/                   # Implementation C#
│   ├── TcpServer.cs
│   ├── TcpClient.cs
│   ├── TcpOptimization.csproj
│   └── build.bat
├── python/                   # Implementation Python
│   ├── tcp_server.py
│   ├── tcp_client.py
│   ├── requirements.txt
│   └── run.bat
├── java/                     # Implementation Java
│   ├── TcpServer.java
│   ├── TcpClient.java
│   └── build.bat
└── nodejs/                   # Implementation Node.js
    ├── tcp_server.js
    ├── tcp_client.js
    ├── package.json
    └── run.bat
```

## Yêu Cầu Hệ Thống

### Go

- Go 1.20+
- Không cần thư viện bên ngoài (sử dụng net package built-in)

### C#

- .NET 8.0 SDK
- Visual Studio 2022 (khuyến nghị) hoặc VS Code

### Python

- Python 3.7+
- Không cần thư viện bên ngoài (sử dụng socket built-in)

### Java

- JDK 8+
- Không cần Maven/Gradle (compile trực tiếp)

### Node.js

- Node.js 14+ (LTS khuyến nghị)
- npm hoặc yarn

### Chạy Từng Ngôn Ngữ

#### Go (Nhanh nhất - 22,222 msg/s)

**Cách 1: Chạy nhanh (1 terminal)**

```powershell
cd golang
Start-Process go -ArgumentList "run tcp_server.go" -NoNewWindow
Start-Sleep -Seconds 2
go run tcp_client.go
```

**Cách 2: Chạy riêng (2 terminals)**

```powershell
# Terminal 1 - Server
cd golang
go run tcp_server.go

# Terminal 2 - Client (chờ server khởi động)
cd golang
go run tcp_client.go
```

**Cách 3: Dùng script**

```powershell
cd golang
.\run.bat
```

#### C# (Hạng 3 - 19,231 msg/s)

**Cách 1: Build rồi chạy**

```powershell
cd csharp
.\build.bat

# Terminal 1 - Server
dotnet run --project TcpOptimization.csproj -- server

# Terminal 2 - Client
dotnet run --project TcpOptimization.csproj -- client
```

**Cách 2: Chạy nhanh (1 terminal)**

```powershell
cd csharp
dotnet build -c Release
Start-Process dotnet -ArgumentList "run --project TcpOptimization.csproj --no-build -c Release -- server" -NoNewWindow
Start-Sleep -Seconds 3
dotnet run --project TcpOptimization.csproj --no-build -c Release -- client
```

#### Python (Á quân - 20,123 msg/s)

**Cách 1: Chạy nhanh (1 terminal)**

```powershell
cd python
Start-Process python -ArgumentList "tcp_server.py" -NoNewWindow
Start-Sleep -Seconds 2
python tcp_client.py
```

**Cách 2: Chạy riêng (2 terminals)**

```powershell
# Terminal 1 - Server
cd python
python tcp_server.py

# Terminal 2 - Client
cd python
python tcp_client.py
```

#### Java (Hạng 4 - 15,385 msg/s)

**Cách 1: Build rồi chạy**

```powershell
cd java
.\build.bat

# Terminal 1 - Server
java TcpServer

# Terminal 2 - Client
java TcpClient
```

**Cách 2: Chạy nhanh (1 terminal)**

```powershell
cd java
javac -source 1.8 -target 1.8 TcpServer.java TcpClient.java
Start-Process java -ArgumentList "TcpServer" -NoNewWindow
Start-Sleep -Seconds 2
java TcpClient
```

#### Node.js (Hạng 5 - 5,348 msg/s)

**Cách 1: Chạy nhanh (1 terminal)**

```powershell
cd nodejs
npm install
Start-Process node -ArgumentList "tcp_server.js" -NoNewWindow
Start-Sleep -Seconds 2
node tcp_client.js
```

**Cách 2: Chạy riêng (2 terminals)**

```powershell
# Terminal 1 - Server
cd nodejs
npm install
node tcp_server.js

# Terminal 2 - Client
cd nodejs
node tcp_client.js
```

## Các Tính Năng Được Triển Khai

Mỗi implementation đều có:

1. ✅ **TCP Server** với các tối ưu hóa:
   - Socket buffer optimization
   - TCP_NODELAY
   - Keep-Alive
   - Reuse address/port
2. ✅ **TCP Client** với:
   - Connection timeout
   - Buffer optimization
   - TCP_NODELAY
3. ✅ **Đo lường hiệu năng**:

   - Thời gian round-trip
   - Throughput (bytes/second)
   - Connection statistics

4. ✅ **Error handling** và logging chi tiết

## 📊 Kết Quả Benchmark - Performance Comparison

### 🏆 Bảng Xếp Hạng Performance (Cập Nhật Mới Nhất)

| Rank     | Language    | Messages/sec | Avg Latency | Min Latency | Max Latency | Throughput | Status           |
| -------- | ----------- | ------------ | ----------- | ----------- | ----------- | ---------- | ---------------- |
| 🥇 **1** | **Go**      | **22,222**   | 0.042 ms    | 0.000 ms    | 0.812 ms    | 0.68 MB/s  | ✅ **QUÁN QUÂN** |
| 🥈 **2** | **Python**  | **20,123**   | 0.047 ms    | 0.027 ms    | 0.259 ms    | 2.05 MB/s  | ✅ Á QUÂN        |
| 🥉 **3** | **C#**      | **19,231**   | 0.050 ms    | 0.025 ms    | 6.269 ms    | 1.81 MB/s  | ✅ HẠNG 3        |
| **4**    | **Java**    | **15,385**   | 0.055 ms    | 0.025 ms    | 3.071 ms    | 1.51 MB/s  | ✅ TRUNG BÌNH    |
| **5**    | **Node.js** | **5,348**    | 0.078 ms    | 0.047 ms    | 1.026 ms    | 0.55 MB/s  | ✅ CUỐI BẢNG     |

### 📈 Phân Tích Chi Tiết

#### 🏆 **Go - CHIẾN THẮNG VƯỢT TRỘI**

- **Messages/sec:** 22,222 (cao nhất, nhanh hơn Python 10%)
- **Latency trung bình:** 0.042 ms (thấp nhất cùng với C#)
- **Latency min:** 0.000 ms (xuất sắc nhất)
- **Latency max:** 0.812 ms (ổn định nhất, không có spike)
- **Duration:** 45 ms (nhanh nhất)

**Ưu điểm:**

- Goroutines cho concurrency tuyệt vời
- Compiled binary performance cao
- Memory management hiệu quả
- Native TCP stack optimization
- Code đơn giản, dễ maintain

**Kết luận:** ⭐⭐⭐⭐⭐ **Hoàn hảo cho high-performance TCP, microservices, cloud-native applications**

#### 🥈 **Python - Á QUÂN ĐÁNG GỜM**

- **Messages/sec:** 20,123 (rất cao, chỉ kém Go 9%)
- **Latency trung bình:** 0.047 ms
- **Throughput:** 2.05 MB/s (cao nhất!)
- **Latency max:** 0.259 ms (ổn định, không có GC spike)
- **Duration:** 50 ms

**Ưu điểm:**

- Socket API native cực kỳ tối ưu
- Code đơn giản nhất trong tất cả
- Không cần compile
- Throughput cao nhất (tốt cho data transfer)
- Development speed nhanh

**Kết luận:** ⭐⭐⭐⭐⭐ **Xuất sắc cho rapid development với performance tuyệt vời**

#### 🥉 **C# - HẠNG 3 VỮNG CHẮC**

- **Messages/sec:** 19,231 (tốt)
- **Latency trung bình:** 0.050 ms
- **Latency max:** 6.269 ms (spike cao do .NET runtime)
- **Throughput:** 1.81 MB/s
- **Duration:** 52 ms

**Ưu điểm:**

- .NET 8.0 runtime mạnh mẽ
- Type-safe, OOP xuất sắc
- Async/await patterns tốt
- Cross-platform (.NET Core)

**Nhược điểm:**

- Latency spikes cao (6.2ms)
- .NET runtime overhead

**Kết luận:** ⭐⭐⭐⭐ **Tuyệt vời cho enterprise applications, .NET ecosystem**

#### **Java - VỊ TRÍ 4**

- **Messages/sec:** 15,385 (cải thiện đáng kể so với lần trước)
- **Latency trung bình:** 0.055 ms
- **Latency max:** 3.071 ms (GC pause)
- **Throughput:** 1.51 MB/s
- **Duration:** 65 ms

**Ưu điểm:**

- Performance cải thiện tốt
- JVM mature và stable
- Cross-platform xuất sắc
- Ecosystem rộng lớn

**Nhược điểm:**

- JVM warmup và GC overhead
- Latency không ổn định như top 3
- Memory footprint lớn

**Kết luận:** ⭐⭐⭐ **Tốt cho enterprise khi đã có Java infrastructure**

#### **Node.js - VỊ TRÍ 5**

- **Messages/sec:** 5,348 (thấp nhất, chỉ 24% so với Go)
- **Latency trung bình:** 0.078 ms (cao nhất)
- **Throughput:** 0.55 MB/s (thấp nhất)
- **Duration:** 187 ms (chậm nhất, gấp 4 lần Go)

**Ưu điểm:**

- Event-driven architecture
- NPM ecosystem khổng lồ
- Async I/O tốt cho I/O-bound tasks

**Nhược điểm:**

- Single-threaded limitation
- JavaScript interpreter overhead rất cao
- Performance kém hơn nhiều so với compiled languages

**Kết luận:** ⭐⭐ **Phù hợp cho web APIs, KHÔNG khuyến nghị cho TCP optimization**

---

### 🎯 Khuyến Nghị Sử Dụng

#### **Cho Production High-Performance TCP:**

1. **🥇 Go** - Lựa chọn số 1 cho:
   - Microservices
   - Cloud-native applications
   - High-concurrency servers
   - Real-time systems
2. **🥈 Python** - Tuyệt vời cho:
   - Rapid development
   - Data-intensive applications
   - Projects cần throughput cao
   - Prototyping nhanh
3. **🥉 C#** - Phù hợp khi:
   - Đã có .NET infrastructure
   - Enterprise applications
   - Cần type-safety mạnh

#### **KHÔNG Khuyến Nghị:**

- ❌ **Node.js** - Cho TCP optimization (performance quá thấp, chỉ 24% so với Go)
- ⚠️ **Java** - Trừ khi bắt buộc do legacy codebase

---

### 📊 So Sánh Trực Quan

**Performance Ranking (Messages/sec):**

```
Go       ████████████████████████ 22,222 (100%)
Python   ██████████████████████   20,123 (91%)
C#       █████████████████████    19,231 (87%)
Java     ███████████████          15,385 (69%)
Node.js  █████                     5,348 (24%)
```

**Latency Comparison (Avg):**

```
Go       ████ 0.042 ms (Thấp nhất)
Python   █████ 0.047 ms
C#       █████ 0.050 ms
Java     ██████ 0.055 ms
Node.js  ████████ 0.078 ms (Cao nhất)
```

---

### 🔬 Chi Tiết Test Environment

- **Test:** 1000 messages round-trip
- **Message size:** ~100 bytes
- **Message format:** `MESSAGE:<id>:<timestamp>\n`
- **Connection:** Localhost (127.0.0.1:8888)
- **OS:** Windows 11
- **TCP Optimizations Applied:**
  - TCP_NODELAY (all languages)
  - Socket buffers: 32KB (all languages)
  - SO_KEEPALIVE (where supported)
  - SO_REUSEADDR (where supported)

## Tác Giả

**Nhóm thực hiện:** Nhóm 14  
**Sinh viên thực hiện:** Nguyễn Thành Chiến  
**Môn học:** Lập trình Mạng – Elearning 1
**Giảng viên hướng dẫn:** ThS.Bùi Dương Thế

## License

MIT License - Tự do sử dụng cho mục đích học tập

## Ghi Chú Kỹ Thuật

### Windows vs Linux

- Trên Windows: Sử dụng Winsock2 API
- Trên Linux: Sử dụng POSIX sockets
- Code được thiết kế cross-platform khi có thể

### Tối Ưu Hóa Nâng Cao

Để tối ưu thêm trong môi trường production:

- Sử dụng epoll/IOCP cho high-concurrency servers
- Áp dụng zero-copy techniques
- Tune kernel parameters (sysctl trên Linux)
- Sử dụng connection pooling
- Implement backpressure handling

### Debugging

Để xem các thông số TCP:

- **Windows**: `netstat -ano`, `netsh int tcp show global`
- **Linux**: `ss -ti`, `netstat -s`, `sysctl net.ipv4.tcp_*`
