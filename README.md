# UDP Protocol Optimization - Elearning 2

Ứng dụng mô phỏng **kỹ thuật tối ưu hóa giao thức UDP** cho môn học _Lập trình Mạng (Elearning-2)_.

---

## Giới thiệu

UDP là giao thức truyền tải nhanh nhưng không đảm bảo độ tin cậy. Trong dự án này, chúng ta xây dựng mô phỏng **các kỹ thuật tối ưu hóa UDP** nhằm cải thiện hiệu suất và đảm bảo dữ liệu, bao gồm:

- ** Packet Bundling** – Gộp nhiều message thành một gói UDP để giảm overhead.
- ** Selective Retransmission** – Chỉ gửi lại gói bị mất thay vì toàn bộ.
- ** ACK-based Reliability** – Sử dụng ACK để xác nhận từng gói tin.
- ** Timeout & RTT Estimation** – Ước lượng thời gian chờ dựa trên RTT.
- ** Loss Detection & Handling** – Mô phỏng mất gói và xử lý thông minh.
- ** Sequence Numbering & Duplicate Prevention** – Đánh số gói để đảm bảo thứ tự và tránh trùng lặp.

---

## Cấu trúc Project

```
Elearning-2/
│
├── src/
│   ├── optimized_udp_server.py   # Server UDP tối ưu hóa
│   ├── optimized_udp_client.py   # Client UDP tối ưu hóa
│   └── demo_optimization.py      # File chạy demo tổng hợp
│
├── README.md                     # Tài liệu mô tả dự án
└── .gitignore
```

---

## Yêu cầu hệ thống

- Python >= 3.8
- Git
- Môi trường: Linux / MacOS / Windows

---

## Cài đặt & Chạy thử

### 1. Clone dự án

```bash
git clone https://github.com/NguyenThanhChien-2010/elearning_LTMang.git
cd elearning_LTMang
```

### 2. Di chuyển vào thư mục dự án

```bash
cd Nhom14-Elearning2/Elearning-2
```

### 3. Chạy Demo

**Windows:**

```powershell
# Nếu Python đã có trong PATH
python src/demo_optimization.py

# Hoặc dùng đường dẫn đầy đủ
C:\Users\<TenUser>\AppData\Local\Programs\Python\Python312\python.exe src/demo_optimization.py
```

**Linux/MacOS:**

```bash
python3 src/demo_optimization.py
```

🎬 **Demo sẽ minh họa toàn bộ các kỹ thuật tối ưu hóa UDP**.

### 4. Chạy thủ công từng phần

**Terminal 1 - Khởi động Server:**

```bash
python src/optimized_udp_server.py
```

**Terminal 2 - Khởi động Client:**

```bash
python src/optimized_udp_client.py
```

**Lưu ý:** Trên Windows, thay `python` bằng đường dẫn đầy đủ nếu cần.

---

## Thống kê trong Demo

Trong quá trình chạy, ứng dụng sẽ hiển thị:

- Số lượng gói tin đã gửi và đã nhận.
- Số lượng ACK được xác nhận.
- Tỉ lệ mất gói & gói bị loại bỏ do duplicate.
- RTT trung bình & tỉ lệ thành công.
- Trạng thái gói chưa được ACK.

---

## Minh họa các kỹ thuật đã cài đặt

✔️ **Packet Bundling** – Gửi nhiều tin nhắn trong một gói UDP.  
✔️ **Selective Retransmission** – Chỉ gửi lại gói mất.  
✔️ **ACK-based Reliability** – Server gửi ACK cho client.  
✔️ **Loss Detection** – Mô phỏng mất gói với xác suất.  
✔️ **Sequence Numbering** – Đảm bảo thứ tự, tránh duplicate.

---

## Ghi chú học tập

- UDP mặc định _không đảm bảo tin cậy_ → cần bổ sung kỹ thuật.
- Các tối ưu hóa này giúp UDP gần giống TCP về độ tin cậy nhưng vẫn nhanh hơn nhờ tính **connectionless**.
- Demo này chỉ mang tính **mô phỏng học thuật**, chưa áp dụng trong môi trường sản xuất thực tế.

---

## Tác giả

- **Nhóm thực hiện:** Nhóm 14
- **Sinh viên thực hiện:** Nguyễn Thành Chiến
- **Môn học:** Lập trình Mạng – Elearning-2
- **GVHD:** Bùi Dương Thế

---

## Giấy phép

Dự án phát hành theo **MIT License** – tự do sử dụng cho mục đích học tập và nghiên cứu.
