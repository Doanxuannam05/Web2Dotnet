using DemoAPINET.DTOs;
using DemoAPINET.DTOs.Auth;

namespace DemoAPINET.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<(bool IsSuccess, string Message, UserDto? Data)> RegisterAsync(RegisterDto request);
        Task<(bool IsSuccess, string Message)> UpdateAsync(int id, RegisterDto request);
        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);
        Task<PagedResponse<UserDto>> GetPagedAsync(UserQueryParameters queryParams);
    }
}
