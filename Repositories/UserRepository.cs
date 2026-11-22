using DemoAPINET.Data;
using DemoAPINET.DTOs;
using DemoAPINET.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoAPINET.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string username, string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username || u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
    }

    public void Delete(User user)
    {
        _db.Users.Remove(user);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
    public async Task<(List<User> Users, int TotalRecords)> GetPagedAsync(UserQueryParameters queryParams)
    {
        var query = _db.Users.AsQueryable();

        // 1. FILTERING: Lọc chính xác theo Role
        if (!string.IsNullOrWhiteSpace(queryParams.Role))
        {
            query = query.Where(u => u.Role == queryParams.Role.Trim());
        }

        // 2. SEARCHING: Tìm kiếm theo UserName hoặc Email (LIKE '%keyword%')
        if (!string.IsNullOrWhiteSpace(queryParams.Keyword))
        {
            var kw = queryParams.Keyword.Trim();
            query = query.Where(u => u.UserName.Contains(kw) || u.Email.Contains(kw));
        }

        // Đếm tổng số bản ghi thỏa điều kiện lọc/tìm kiếm trước khi cắt trang
        var totalRecords = await query.CountAsync();

        // 3. SORTING: Sắp xếp theo Whitelist các trường
        query = queryParams.SortBy?.ToLower() switch
        {
            "username" => queryParams.IsDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),

            "email" => queryParams.IsDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            _ => queryParams.IsDescending
                ? query.OrderByDescending(u => u.Id)
                : query.OrderBy(u => u.Id)
        };

        // 4. PAGINATION: Skip/Take
        var skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
        var users = await query
            .Skip(skip)
            .Take(queryParams.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (users, totalRecords);
    }
}
