using DemoAPINET.Models;

namespace DemoAPINET.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
