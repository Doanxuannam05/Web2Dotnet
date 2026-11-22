using DemoAPINET.DTOs;
using DemoAPINET.Models;

namespace DemoAPINET.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email);
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task<bool> SaveChangesAsync();

    // Bổ sung dòng này để sửa 2 lỗi CS1061 của AuthController & AuthService:
    Task<User?> GetByUserNameAsync(string userName);

    // Bổ sung dòng này để sửa lỗi ở UserService:
    Task<(List<User> Users, int TotalRecords)> GetPagedAsync(UserQueryParameters queryParams);
}