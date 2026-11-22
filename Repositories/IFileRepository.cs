// Repositories/IFileRepository.cs
using DemoAPINET.Models;

namespace DemoAPINET.Repositories
{
    public interface IFileRepository
    {
        Task<StoredFile> AddAsync(StoredFile file);
        Task<StoredFile?> GetByIdAsync(int id);
        Task<IEnumerable<StoredFile>> GetAllAsync();
    }
}