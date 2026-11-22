# HR Leave Management System - Hệ thống Quản lý Ngày nghỉ Nhân sự

Dự án Web API xây dựng trên nền tảng **ASP.NET Core (.NET 10)** kết hợp giao diện **Frontend Single Page Application (SPA) phong cách 3D Claymorphism & Motion Design**, quản lý toàn diện quy trình nộp đơn, xét duyệt và thống kê ngày nghỉ phép cho doanh nghiệp.

---

## 🌟 Tính năng nổi bật

* **Phân quyền và nhận diện đuôi tài khoản thông minh**:
  * 👤 **Nhân viên (`.nv`)**: Tự động gán quyền `Employee`, quản lý hạn mức phép cá nhân (12 ngày phép tiêu chuẩn), nộp đơn xin nghỉ, hủy đơn chờ duyệt và theo dõi lịch sử.
  * 💼 **Nhân sự / Quản lý (`.hr`)**: Gán quyền `HR`, xét duyệt hoặc từ chối đơn nghỉ phép kèm lý do, tự động trừ ngày phép còn lại khi duyệt.
  * 👑 **Quản trị viên (`.ad`)**: Gán quyền `Admin`, toàn quyền quản trị danh mục phòng ban, loại ngày nghỉ và hồ sơ nhân sự.
* **Quy trình nghiệp vụ chặt chẽ**:
  * Kiểm tra trùng lặp khoảng thời gian xin nghỉ.
  * Kiểm tra và cảnh báo vượt hạn mức ngày nghỉ phép khả dụng theo thời gian thực.
  * Tự động hoàn trả ngày phép nếu hủy hoặc từ chối đơn.
* **Báo cáo thống kê tự động thích ứng theo vai trò**:
  * *Nhân viên*: Xem báo cáo ngày nghỉ cá nhân (Tổng số đơn, số ngày đã duyệt, phân loại theo loại nghỉ).
  * *Quản lý/Admin*: Xem báo cáo tổng hợp toàn doanh nghiệp và chi tiết số ngày nghỉ theo từng phòng ban.
  * Hỗ trợ xuất dữ liệu ra file Excel/CSV nhanh chóng.
* **Giao diện 3D Claymorphism & Web Motion**:
  * Bảng màu cao cấp chuẩn editorial: `#3D52A0`, `#7091E6`, `#8697C4`, `#ADBBDA`, `#EDE8F5`.
  * Nút bấm nam châm hút chuột (Magnetic buttons), thẻ nổi khối 3D Parallax Tilt, thanh tiến trình hạn mức 3D.
  * Chuyển đổi nhanh vai trò 1-chạm (Quick Switcher) thuận tiện cho việc thuyết trình và kiểm thử.
* **Chuẩn kiến trúc phần mềm**:
  * Kiến trúc phân tầng **N-Tier Architecture** kết hợp **Repository & Service Pattern**.
  * **Global Exception Handling Middleware**: Bắt lỗi tập trung, trả mã lỗi HTTP 500 JSON chuẩn.
  * **Request Timing Middleware**: Đo lường và log hiệu năng thời gian xử lý từng request (ms).
  * **Swagger UI / OpenAPI 3.0** tích hợp Bearer JWT Authentication.
  * Hỗ trợ **Docker & Docker Compose** đóng gói và chạy trọn gói.

---

## 🔑 Tài khoản mẫu có sẵn (1-Click Login)

Hệ thống đã tự động nạp sẵn (Seed Data) các tài khoản mẫu để bạn kiểm thử ngay:

| Vai trò | Tên đăng nhập | Mật khẩu | Quyền hạn |
| :--- | :--- | :--- | :--- |
| 👑 **Quản trị viên** | `admin.ad` *(hoặc `admin`)* | `Admin@123` | Toàn quyền hệ thống |
| 💼 **Quản lý / HR** | `manager.hr` | `Hr@123` | Phê duyệt đơn, xem thống kê toàn công ty |
| 👤 **Nhân viên** | `hoanganh.nv` | `User@123` | Nộp đơn, theo dõi phép cá nhân |

---

## 🚀 Hướng dẫn Cài đặt & Chạy dự án

### Cách 1: Chạy bằng Docker (Khuyên dùng - Nhanh nhất)

Yêu cầu máy tính đã cài đặt **[Docker Desktop](https://www.docker.com/products/docker-desktop/)**.

#### Bước 1: Clone dự án về máy
Mở Terminal hoặc Command Prompt và chạy:
```bash
git clone https://github.com/Doanxuannam05/Web2Dotnet.git
cd Web2Dotnet
```

#### Bước 2: Khởi động hệ thống với Docker Compose
Chạy lệnh sau để tự động tải SQL Server, build ứng dụng và khởi chạy toàn bộ:
```bash
docker compose up -d --build
```

#### Bước 3: Truy cập ứng dụng
Sau khi container khởi động xong, mở trình duyệt:
* 🌐 **Giao diện Web 3D**: [http://localhost:5091](http://localhost:5091)
* 📑 **Tài liệu Swagger API**: [http://localhost:5091/swagger](http://localhost:5091/swagger)

*Để dừng hệ thống:*
```bash
docker compose down
```

---

### Cách 2: Chạy trực tiếp trên Visual Studio / .NET CLI

Yêu cầu máy tính đã cài đặt:
* **[.NET 10 SDK](https://dotnet.microsoft.com/download)**
* **[Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)** (Local instance `localhost\MSSQL2026` hoặc SQL Express)
* **Visual Studio 2026 / 2022** (hỗ trợ .NET 10)

#### Bước 1: Clone dự án
```bash
git clone https://github.com/Doanxuannam05/Web2Dotnet.git
cd Web2Dotnet
```

#### Bước 2: Cấu hình chuỗi kết nối Database
Mở file `appsettings.json` và điều chỉnh chuỗi kết nối phù hợp với máy của bạn:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=localhost\\MSSQL2026;Initial Catalog=mydatabase;Integrated Security=True;TrustServerCertificate=True;"
}
```

#### Bước 3: Khởi chạy dự án
* **Cách A (Bằng Visual Studio)**:
  * Mở file `DemoAPINET.sln`.
  * Nhấn **`Ctrl + F5`** (Start Without Debugging).
  * Ứng dụng sẽ tự động chạy migration tạo cơ sở dữ liệu và mở trình duyệt tại `http://localhost:5091`.

* **Cách B (Bằng dòng lệnh .NET CLI)**:
  ```bash
  dotnet restore
  dotnet run --project DemoAPINET.csproj
  ```

---

### Cách 3: Build Docker Image riêng lẻ

Nếu bạn chỉ muốn đóng gói riêng Web API:
```bash
# 1. Build image
docker build -t demoapinet-app .

# 2. Chạy container
docker run -d -p 5091:8080 --name demoapi_instance demoapinet-app
```

---

## 🏛️ Kiến trúc dự án (Project Structure)

```text
Web2Dotnet/
├── Controllers/            # API Controllers (Auth, LeaveRequests, Employees,...)
├── Data/                   # DbContext, DataSeeder khởi tạo dữ liệu mẫu
├── DTOs/                   # Data Transfer Objects kiểm soát dữ liệu vào/ra
├── Middleware/             # GlobalExceptionMiddleware, RequestTimingMiddleware
├── Migrations/             # EF Core Code-First Migrations
├── Models/                 # Database Entities (User, Employee, LeaveRequest,...)
├── Repositories/           # Data Access Layer (Repository Pattern)
├── Services/               # Business Logic Layer (Service Pattern)
├── wwwroot/                # Frontend SPA 3D (HTML, Tailwind CSS, JavaScript)
│   └── index.html          # Giao diện chính hệ thống
├── Dockerfile              # Multi-stage build Docker cho .NET 10
├── docker-compose.yml      # Cấu hình khởi chạy App + SQL Server
└── Program.cs              # Dependency Injection và Middleware Pipeline
```

---

## 🛠️ Công nghệ sử dụng

* **Backend**: ASP.NET Core Web API (.NET 10 C#), Entity Framework Core 10.
* **Database**: Microsoft SQL Server.
* **Authentication**: JWT (JSON Web Token), mã hóa mật khẩu `HMAC-SHA512` kết hợp `Salt` ngẫu nhiên 128-byte.
* **Frontend**: HTML5, Tailwind CSS, Axios, Google Fonts (Plus Jakarta Sans).
* **Motion & 3D**: CSS 3D Transforms, Spring Physics Easing, Magnetic Buttons.
* **DevOps & Container**: Docker, Docker Compose, Swagger UI / OpenAPI 3.0.
