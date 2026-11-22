# BACKEND ARCHITECTURE
## HR Leave Management System

### 1. Giới thiệu

HR Leave Management System là hệ thống quản lý ngày nghỉ phép nhân sự được xây dựng bằng ASP.NET Core Web API trên nền tảng .NET 10.

Hệ thống hỗ trợ quản lý nhân viên, phòng ban, loại nghỉ phép, đơn xin nghỉ, quy trình xét duyệt và xác thực người dùng.

Backend được tổ chức theo kiến trúc phân tầng, sử dụng Repository Pattern, Service Pattern và Dependency Injection nhằm tách biệt trách nhiệm giữa các thành phần.

### 2. Công nghệ sử dụng

| Công nghệ | Vai trò |
|---|---|
| ASP.NET Core .NET 10 | Xây dựng Web API |
| C# | Ngôn ngữ lập trình |
| Entity Framework Core 10 | ORM truy cập dữ liệu |
| SQL Server | Hệ quản trị cơ sở dữ liệu |
| JWT Bearer | Xác thực người dùng |
| BCrypt.Net | Hỗ trợ mã hóa mật khẩu |
| Swagger / OpenAPI | Tài liệu và kiểm thử API |
| Docker | Đóng gói và triển khai ứng dụng |

### 3. Cấu trúc Backend

Các thư mục chính:

- `Controllers`: Tiếp nhận và xử lý HTTP requests.
- `Data`: Chứa AppDbContext và DataSeeder.
- `DTOs`: Định nghĩa các đối tượng truyền dữ liệu.
- `Middleware`: Xử lý ngoại lệ và đo thời gian request.
- `Migrations`: Lưu lịch sử thay đổi cấu trúc cơ sở dữ liệu.
- `Models`: Các lớp mô hình dữ liệu.
- `Repositories`: Thực hiện truy cập và thao tác dữ liệu.
- `Services`: Xử lý logic nghiệp vụ.
- `wwwroot`: Chứa tài nguyên tĩnh và giao diện ứng dụng.

### 4. Kiến trúc phân tầng

Hệ thống áp dụng mô hình phân tầng:

1. Controller Layer: Nhận request từ client, gọi Service và trả HTTP response.
2. Service Layer: Xử lý nghiệp vụ và điều phối các thao tác dữ liệu.
3. Repository Layer: Thực hiện truy vấn và cập nhật dữ liệu thông qua Entity Framework Core.
4. Data Layer: Quản lý kết nối và ánh xạ dữ liệu thông qua AppDbContext.
5. Database Layer: Lưu trữ dữ liệu trên SQL Server.

Luồng xử lý cơ bản:

Client → Controller → Service → Repository → AppDbContext → SQL Server.

Kết quả được trả ngược từ Backend về Client dưới dạng HTTP response.

### 5. Dependency Injection

Các Repository và Service được đăng ký trong `Program.cs` bằng phương thức `AddScoped`.

Một số thành phần tiêu biểu:

- IUserRepository / UserRepository
- IUserService / UserService
- IAuthService / AuthService
- IDepartmentRepository / DepartmentRepository
- IDepartmentService / DepartmentService
- IEmployeeRepository / EmployeeRepository
- IEmployeeService / EmployeeService
- ILeaveRequestRepository / LeaveRequestRepository
- ILeaveTypeRepository / LeaveTypeRepository
- ILeaveService / LeaveService

Việc sử dụng Dependency Injection giúp giảm sự phụ thuộc trực tiếp giữa các lớp và hỗ trợ bảo trì, kiểm thử.

### 6. Kết nối cơ sở dữ liệu

Backend sử dụng Entity Framework Core và SQL Server.

`AppDbContext` được đăng ký thông qua `AddDbContext` trong `Program.cs`.

Chuỗi kết nối được đọc từ cấu hình:

`ConnectionStrings:DefaultConnection`.

Ngoài ra, ứng dụng cấu hình `EnableRetryOnFailure` với tối đa 3 lần thử lại khi gặp một số lỗi kết nối có thể phục hồi.

Các thay đổi cấu trúc dữ liệu được quản lý thông qua thư mục `Migrations`.

### 7. Xác thực và phân quyền

Hệ thống sử dụng JWT Bearer Authentication.

Quá trình xác thực bao gồm:

1. Người dùng gửi thông tin đăng nhập.
2. Backend kiểm tra thông tin tài khoản.
3. Nếu hợp lệ, hệ thống cấp JWT Token.
4. Client gửi token trong HTTP Authorization Header.
5. Backend xác thực token trước khi cho phép truy cập tài nguyên được bảo vệ.

JWT được cấu hình kiểm tra:

- Issuer.
- Audience.
- Token Lifetime.
- Signing Key.

Hệ thống sử dụng Role Claim để hỗ trợ phân quyền theo vai trò.

### 8. Middleware

Backend đăng ký hai middleware tùy chỉnh:

**GlobalExceptionMiddleware**

- Tiếp nhận các ngoại lệ phát sinh trong request pipeline.
- Tập trung xử lý lỗi.
- Trả về phản hồi lỗi theo định dạng JSON.

**RequestTimingMiddleware**

- Đo thời gian xử lý request.
- Ghi nhận thời gian thực thi theo milliseconds.
- Hỗ trợ theo dõi hiệu năng ứng dụng.

Hai middleware được đăng ký trước các thành phần xử lý request khác trong pipeline.

### 9. Swagger và CORS

Swagger được cấu hình để mô tả API và hỗ trợ kiểm thử trực tiếp.

Hệ thống khai báo cơ chế bảo mật Bearer JWT, cho phép nhập token để thử nghiệm các API yêu cầu xác thực.

CORS sử dụng policy `AllowFrontend`, hiện cho phép frontend tại:

`http://localhost:3000`

Policy cho phép các HTTP headers và HTTP methods cần thiết.

### 10. Khởi tạo dữ liệu mẫu

Trong quá trình khởi động ứng dụng, `DataSeeder.SeedAsync` được gọi để khởi tạo dữ liệu mẫu.

Các nhóm dữ liệu được ghi nhận trong cấu hình gồm:

- Tài khoản Admin.
- Tài khoản HR.
- Tài khoản nhân viên.
- Loại nghỉ phép.
- Phòng ban.

Việc khởi tạo dữ liệu mẫu hỗ trợ kiểm thử các chức năng của hệ thống.

### 11. Kết luận

Backend được tổ chức theo kiến trúc phân tầng, kết hợp Dependency Injection, Repository Pattern và Service Pattern.

Các công nghệ ASP.NET Core, Entity Framework Core, SQL Server và JWT giúp hệ thống có cấu trúc rõ ràng, hỗ trợ mở rộng và thuận tiện trong quá trình phát triển, kiểm thử và bảo trì.