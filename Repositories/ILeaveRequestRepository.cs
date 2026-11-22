using DemoAPINET.DTOs.Leave;
using DemoAPINET.Models;

namespace DemoAPINET.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<(List<LeaveRequest> Items, int TotalCount)> GetPagedAsync(LeaveQueryParameters query);
        Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime start, DateTime end, int? excludeId = null);
        Task AddAsync(LeaveRequest request);
        void Update(LeaveRequest request);
        Task<bool> SaveChangesAsync();
        Task<LeaveStatisticsDto> GetStatisticsAsync(int? employeeId = null);
    }
}
