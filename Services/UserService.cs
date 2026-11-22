using DemoAPINET.DTOs;
using DemoAPINET.DTOs.Auth;
using DemoAPINET.Models;
using DemoAPINET.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace DemoAPINET.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<(bool IsSuccess, string Message, UserDto? Data)> RegisterAsync(
            RegisterDto request)
        {
            var existingUser =
                await _userRepo.GetByUsernameOrEmailAsync(
                    request.UserName,
                    request.Email
                );

            if (existingUser != null)
            {
                return (
                    false,
                    "Username hoặc Email đã được sử dụng.",
                    null
                );
            }

            var passwordData = CreatePasswordHash(request.Password);

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,

                PasswordHash = passwordData.Hash,
                PasswordSalt = passwordData.Salt,

                Role = string.IsNullOrEmpty(request.Role)
                    ? "User"
                    : request.Role,

                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync();

            return (
                true,
                "Đăng ký thành công",
                MapToDto(newUser)
            );
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(
            int id,
            RegisterDto request)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return (
                    false,
                    $"Không tìm thấy người dùng có Id = {id}"
                );
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                user.Role = request.Role;
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                user.UserName = request.UserName;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                var passwordData = CreatePasswordHash(
                    request.Password
                );

                user.PasswordHash = passwordData.Hash;
                user.PasswordSalt = passwordData.Salt;
            }

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return (
                true,
                $"Cập nhật thành công người dùng Id = {id}"
            );
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return (
                    false,
                    $"Không tìm thấy người dùng có Id = {id}"
                );
            }

            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();

            return (
                true,
                $"Đã xóa người dùng Id = {id}"
            );
        }

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            IsActive = true
        };

        private static (
            byte[] Hash,
            byte[] Salt
        ) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();

            var salt = hmac.Key;

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(password)
            );

            return (hash, salt);
        }
        public async Task<PagedResponse<UserDto>> GetPagedAsync(UserQueryParameters queryParams)
        {
            // SAI (gọi nhầm tên class DTO):
            // Sửa từ:
            // var (users, totalRecords) = await UserQueryParameters.GetPagedAsync(queryParams);

            // Thành biến repository (ví dụ nếu đặt tên là _userRepository):
            var (users, totalRecords) = await _userRepo.GetPagedAsync(queryParams);

            // Ánh xạ sang danh sách UserDto để che giấu PasswordHash
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();

            return new PagedResponse<UserDto>(userDtos, totalRecords, queryParams.PageNumber, queryParams.PageSize);
        }
    }
}