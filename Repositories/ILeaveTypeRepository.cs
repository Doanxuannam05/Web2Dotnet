using DemoAPINET.Models;

namespace DemoAPINET.Repositories
{
    public interface ILeaveTypeRepository
    {
        Task<List<LeaveType>> GetAllAsync();
        Task<LeaveType?> GetByIdAsync(int id);
        Task AddAsync(LeaveType leaveType);
        void Update(LeaveType leaveType);
        void Delete(LeaveType leaveType);
        Task<bool> SaveChangesAsync();
    }
}
