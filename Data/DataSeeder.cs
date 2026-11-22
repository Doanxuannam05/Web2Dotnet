using DemoAPINET.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DemoAPINET.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            try
            {
                if (db.Database.IsRelational())
                {
                    try
                    {
                        await db.Database.MigrateAsync();
                    }
                    catch
                    {
                        try { await db.Database.EnsureCreatedAsync(); } catch { }
                    }
                }
                else
                {
                    await db.Database.EnsureCreatedAsync();
                }
            }
            catch { }

            // 1. Seed danh mục Phòng ban
            try
            {
                if (!await db.Departments.AnyAsync())
                {
                    var itDept = new Department { Name = "Phòng Công nghệ Thông tin", Code = "IT", Description = "Phát triển phần mềm và hạ tầng hệ thống" };
                    var hrDept = new Department { Name = "Phòng Quản trị Nhân sự", Code = "HR", Description = "Quản lý chế độ, chính sách và nhân sự" };
                    var salesDept = new Department { Name = "Phòng Kinh doanh", Code = "SALES", Description = "Phát triển thị trường và khách hàng" };

                    await db.Departments.AddRangeAsync(itDept, hrDept, salesDept);
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            // 2. Seed danh mục Loại nghỉ phép
            try
            {
                if (!await db.LeaveTypes.AnyAsync())
                {
                    var leaveTypes = new List<LeaveType>
                    {
                        new() { Name = "Nghỉ phép năm (Annual Leave)", DefaultDays = 12, IsPaid = true, Description = "Nghỉ phép hưởng nguyên lương theo luật lao động" },
                        new() { Name = "Nghỉ ốm đau (Sick Leave)", DefaultDays = 5, IsPaid = true, Description = "Nghỉ điều trị y tế kèm giấy xác nhận của bác sĩ" },
                        new() { Name = "Nghỉ việc riêng (Personal Leave)", DefaultDays = 3, IsPaid = true, Description = "Kết hôn, hiếu hỉ hưởng nguyên lương" },
                        new() { Name = "Nghỉ không lương (Unpaid Leave)", DefaultDays = 30, IsPaid = false, Description = "Nghỉ việc cá nhân không hưởng lương" }
                    };

                    await db.LeaveTypes.AddRangeAsync(leaveTypes);
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            // 3. Seed tài khoản Admin: admin.ad và admin
            User? adminUser = null;
            try
            {
                var (adminHash, adminSalt) = CreatePasswordHash("Admin@123");

                // Đảm bảo admin.ad tồn tại
                adminUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == "admin.ad");
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = "admin.ad",
                        Email = "admin.ad@company.com",
                        PasswordHash = adminHash,
                        PasswordSalt = adminSalt,
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await db.Users.AddAsync(adminUser);
                    await db.SaveChangesAsync();
                }
                else
                {
                    adminUser.PasswordHash = adminHash;
                    adminUser.PasswordSalt = adminSalt;
                    adminUser.Role = "Admin";
                    await db.SaveChangesAsync();
                }

                // Cập nhật hoặc tạo tài khoản admin (phòng trường hợp người dùng gõ admin không có đuôi)
                var oldAdmin = await db.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
                if (oldAdmin == null)
                {
                    await db.Users.AddAsync(new User
                    {
                        UserName = "admin",
                        Email = "admin@company.com",
                        PasswordHash = adminHash,
                        PasswordSalt = adminSalt,
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();
                }
                else
                {
                    oldAdmin.PasswordHash = adminHash;
                    oldAdmin.PasswordSalt = adminSalt;
                    oldAdmin.Role = "Admin";
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            // 4. Seed tài khoản HR: manager.hr
            User? hrUser = null;
            try
            {
                var (hrHash, hrSalt) = CreatePasswordHash("Hr@123");
                hrUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == "manager.hr");
                if (hrUser == null)
                {
                    hrUser = new User
                    {
                        UserName = "manager.hr",
                        Email = "manager.hr@company.com",
                        PasswordHash = hrHash,
                        PasswordSalt = hrSalt,
                        Role = "HR",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await db.Users.AddAsync(hrUser);
                    await db.SaveChangesAsync();
                }
                else
                {
                    hrUser.PasswordHash = hrHash;
                    hrUser.PasswordSalt = hrSalt;
                    hrUser.Role = "HR";
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            // 5. Seed tài khoản Nhân viên: hoanganh.nv
            User? empUser = null;
            try
            {
                var (empHash, empSalt) = CreatePasswordHash("User@123");
                empUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == "hoanganh.nv");
                if (empUser == null)
                {
                    empUser = new User
                    {
                        UserName = "hoanganh.nv",
                        Email = "hoanganh.nv@company.com",
                        PasswordHash = empHash,
                        PasswordSalt = empSalt,
                        Role = "Employee",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await db.Users.AddAsync(empUser);
                    await db.SaveChangesAsync();
                }
                else
                {
                    empUser.PasswordHash = empHash;
                    empUser.PasswordSalt = empSalt;
                    empUser.Role = "Employee";
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            // 6. Liên kết hồ sơ Nhân viên (Employees)
            try
            {
                var itDepartment = await db.Departments.FirstOrDefaultAsync(d => d.Code == "IT");
                var hrDepartment = await db.Departments.FirstOrDefaultAsync(d => d.Code == "HR");

                if (adminUser != null && !await db.Employees.AnyAsync(e => e.UserId == adminUser.Id))
                {
                    await db.Employees.AddAsync(new Employee
                    {
                        UserId = adminUser.Id,
                        FullName = "Quản Trị Viên",
                        Email = adminUser.Email,
                        PhoneNumber = "0901234567",
                        Position = "Giám đốc Công nghệ",
                        DepartmentId = itDepartment?.Id,
                        AnnualLeaveQuota = 12,
                        RemainingLeaveDays = 12
                    });
                }

                if (hrUser != null && !await db.Employees.AnyAsync(e => e.UserId == hrUser.Id))
                {
                    await db.Employees.AddAsync(new Employee
                    {
                        UserId = hrUser.Id,
                        FullName = "Trần Thị HR",
                        Email = hrUser.Email,
                        PhoneNumber = "0902345678",
                        Position = "Trưởng phòng Nhân sự",
                        DepartmentId = hrDepartment?.Id,
                        AnnualLeaveQuota = 12,
                        RemainingLeaveDays = 12
                    });
                }

                if (empUser != null && !await db.Employees.AnyAsync(e => e.UserId == empUser.Id))
                {
                    await db.Employees.AddAsync(new Employee
                    {
                        UserId = empUser.Id,
                        FullName = "Nguyễn Hoàng Anh",
                        Email = empUser.Email,
                        PhoneNumber = "0903456789",
                        Position = "Lập trình viên Backend",
                        DepartmentId = itDepartment?.Id,
                        AnnualLeaveQuota = 12,
                        RemainingLeaveDays = 10
                    });
                }

                await db.SaveChangesAsync();
            }
            catch { }
        }

        private static (byte[] hash, byte[] salt) CreatePasswordHash(string password)
        {
            var salt = new byte[128];
            RandomNumberGenerator.Fill(salt);

            using var hmac = new HMACSHA512(salt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return (hash, salt);
        }
    }
}
