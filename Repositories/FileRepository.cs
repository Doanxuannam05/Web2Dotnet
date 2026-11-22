// Repositories/FileRepository.cs
using DemoAPINET.Data;
using DemoAPINET.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoAPINET.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoredFile> AddAsync(StoredFile file)
        {
            await _context.StoredFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<StoredFile?> GetByIdAsync(int id)
        {
            return await _context.StoredFiles.FindAsync(id);
        }

        public async Task<IEnumerable<StoredFile>> GetAllAsync()
        {
            return await _context.StoredFiles.AsNoTracking().ToListAsync();
        }
    }
}