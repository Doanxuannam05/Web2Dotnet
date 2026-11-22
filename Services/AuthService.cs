using DemoAPINET.DTOs.Auth;
using DemoAPINET.Models;
using DemoAPINET.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoAPINET.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var username = dto.UserName.Trim();
            var password = dto.Password.Trim();

            var user = await _userRepository.GetByUserNameAsync(username);

            // Hỗ trợ alias khi người dùng gõ có hoặc không có đuôi đối với tài khoản mẫu
            if (user == null)
            {
                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("admin.ad");
                else if (username.Equals("admin.ad", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("admin");
                else if (username.Equals("hr", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("manager.hr");
                else if (username.Equals("manager.hr", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("hr");
                else if (username.Equals("employee", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("hoanganh.nv");
                else if (username.Equals("hoanganh.nv", StringComparison.OrdinalIgnoreCase))
                    user = await _userRepository.GetByUserNameAsync("employee");
            }

            // 1. Tự động hỗ trợ khởi tạo nhanh cho các tài khoản mẫu nếu chưa có trong DB
            if (user == null)
            {
                if (username.Equals("admin.ad", StringComparison.OrdinalIgnoreCase) || username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    user = await CreateDemoUserAndEmployeeAsync(username, $"{username}@company.com", "Quản Trị Viên", password, "Admin");
                }
                else if (username.Equals("manager.hr", StringComparison.OrdinalIgnoreCase) || username.Equals("hr", StringComparison.OrdinalIgnoreCase))
                {
                    user = await CreateDemoUserAndEmployeeAsync(username, $"{username}@company.com", "Trần Thị HR", password, "HR");
                }
                else if (username.Equals("hoanganh.nv", StringComparison.OrdinalIgnoreCase) || username.Equals("employee", StringComparison.OrdinalIgnoreCase))
                {
                    user = await CreateDemoUserAndEmployeeAsync(username, $"{username}@company.com", "Nguyễn Hoàng Anh", password, "Employee");
                }
                else
                {
                    return null;
                }
            }

            // 2. Kiểm tra mật khẩu (HMAC-SHA512)
            bool isPasswordCorrect = false;
            try
            {
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                isPasswordCorrect = CryptographicOperations.FixedTimeEquals(computedHash, user.PasswordHash);
            }
            catch { }

            if (!isPasswordCorrect)
            {
                // Hỗ trợ linh hoạt cho các tài khoản mẫu đổi qua lại giữa Admin@123, Password123@, admin, 123456
                bool isDemoAccount = username.Equals("admin.ad", StringComparison.OrdinalIgnoreCase) ||
                                     username.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                                     username.Equals("manager.hr", StringComparison.OrdinalIgnoreCase) ||
                                     username.Equals("hr", StringComparison.OrdinalIgnoreCase) ||
                                     username.Equals("hoanganh.nv", StringComparison.OrdinalIgnoreCase) ||
                                     username.Equals("employee", StringComparison.OrdinalIgnoreCase);

                if (isDemoAccount && (password == "Admin@123" || password == "Hr@123" || password == "User@123" || password == "Password123@" || password == "admin" || password == "123456"))
                {
                    var (newHash, newSalt) = GenerateHashAndSalt(password);
                    user.PasswordHash = newHash;
                    user.PasswordSalt = newSalt;
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }
                else
                {
                    return null;
                }
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token
            };
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto)
        {
            var username = registerDto.UserName.Trim();
            var lowerUser = username.ToLower();

            // 1. Phân loại quyền (Role) dựa theo quy tắc đuôi tên tài khoản: .nv, .hr, .ad
            string role;
            if (lowerUser.EndsWith(".nv"))
            {
                role = "Employee";
            }
            else if (lowerUser.EndsWith(".hr"))
            {
                role = "HR";
            }
            else if (lowerUser.EndsWith(".ad"))
            {
                role = "Admin";
            }
            else
            {
                return (false, "Tên tài khoản không đúng quy định! Bắt buộc phải có đuôi: .nv (Nhân viên), .hr (Nhân sự) hoặc .ad (Quản trị viên). Ví dụ: hoanganh.nv, trang.hr, admin.ad");
            }

            // 2. Kiểm tra trùng lặp
            var existing = await _userRepository.GetByUsernameOrEmailAsync(username, registerDto.Email.Trim());
            if (existing != null)
            {
                return (false, "Tên tài khoản hoặc Email này đã tồn tại trong hệ thống.");
            }

            // 3. Tạo User
            var (hash, salt) = GenerateHashAndSalt(registerDto.Password.Trim());

            var user = new User
            {
                UserName = username,
                Email = registerDto.Email.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            var saved = await _userRepository.SaveChangesAsync();
            if (!saved)
            {
                return (false, "Lỗi khi lưu tài khoản vào cơ sở dữ liệu.");
            }

            // 4. Tự động khởi tạo hồ sơ Nhân sự (Employee) cho tài khoản này
            try
            {
                var fullName = !string.IsNullOrWhiteSpace(registerDto.FullName)
                    ? registerDto.FullName.Trim()
                    : username;

                var position = role == "Admin" ? "Quản trị viên" :
                               role == "HR" ? "Chuyên viên Nhân sự" : "Nhân viên";

                var employee = new Employee
                {
                    UserId = user.Id,
                    FullName = fullName,
                    Email = user.Email,
                    PhoneNumber = registerDto.PhoneNumber?.Trim(),
                    Position = position,
                    HireDate = DateTime.UtcNow,
                    AnnualLeaveQuota = 12,
                    RemainingLeaveDays = 12
                };

                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync();
            }
            catch { }

            return (true, $"Đăng ký thành công tài khoản {username} (Quyền: {role})!");
        }

        private async Task<User> CreateDemoUserAndEmployeeAsync(string username, string email, string fullName, string password, string role)
        {
            var (hash, salt) = GenerateHashAndSalt(password);
            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            try
            {
                var employee = new Employee
                {
                    UserId = user.Id,
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = "0901234567",
                    Position = role == "Admin" ? "Giám đốc Công nghệ" : role == "HR" ? "Trưởng phòng Nhân sự" : "Lập trình viên Backend",
                    HireDate = DateTime.UtcNow,
                    AnnualLeaveQuota = 12,
                    RemainingLeaveDays = 12
                };
                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync();
            }
            catch { }

            return user;
        }

        private static (byte[] hash, byte[] salt) GenerateHashAndSalt(string password)
        {
            var salt = new byte[128];
            RandomNumberGenerator.Fill(salt);

            using var hmac = new HMACSHA512(salt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return (hash, salt);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = Encoding.UTF8.GetBytes(
                jwtSettings["SecretKey"]!
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(secretKey);

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["ExpiryMinutes"] ?? "60")
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
