using DemoAPINET.Models;

namespace DemoAPINET.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByUserIdAsync(int userId);
        Task AddAsync(Employee employee);
        void Update(Employee employee);
        void Delete(Employee employee);
        Task<bool> SaveChangesAsync();
    }
}
