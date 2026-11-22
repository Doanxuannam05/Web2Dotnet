using DemoAPINET.Data;
using DemoAPINET.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoAPINET.Repositories
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly AppDbContext _context;

        public LeaveTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveType>> GetAllAsync()
        {
            return await _context.LeaveTypes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveType?> GetByIdAsync(int id)
        {
            return await _context.LeaveTypes
                .FirstOrDefaultAsync(lt => lt.Id == id);
        }

        public async Task AddAsync(LeaveType leaveType)
        {
            await _context.LeaveTypes.AddAsync(leaveType);
        }

        public void Update(LeaveType leaveType)
        {
            _context.LeaveTypes.Update(leaveType);
        }

        public void Delete(LeaveType leaveType)
        {
            _context.LeaveTypes.Remove(leaveType);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
