using DemoAPINET.Data;
using DemoAPINET.DTOs.Leave;
using DemoAPINET.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoAPINET.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<LeaveRequest> Items, int TotalCount)> GetPagedAsync(LeaveQueryParameters query)
        {
            var q = _context.LeaveRequests
                .Include(lr => lr.Employee).ThenInclude(e => e!.Department)
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.ApprovedByUser)
                .Include(lr => lr.AttachedFile)
                .AsQueryable();

            if (query.EmployeeId.HasValue)
            {
                q = q.Where(lr => lr.EmployeeId == query.EmployeeId.Value);
            }

            if (query.DepartmentId.HasValue)
            {
                q = q.Where(lr => lr.Employee != null && lr.Employee.DepartmentId == query.DepartmentId.Value);
            }

            if (query.Status.HasValue)
            {
                q = q.Where(lr => lr.Status == query.Status.Value);
            }

            if (query.FromDate.HasValue)
            {
                q = q.Where(lr => lr.StartDate >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                q = q.Where(lr => lr.EndDate <= query.ToDate.Value);
            }

            var totalCount = await q.CountAsync();

            var skip = (query.PageNumber - 1) * query.PageSize;
            var items = await q
                .OrderByDescending(lr => lr.CreatedAt)
                .Skip(skip)
                .Take(query.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.ApprovedByUser)
                .Include(lr => lr.AttachedFile)
                .Where(lr => lr.EmployeeId == employeeId)
                .OrderByDescending(lr => lr.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee).ThenInclude(e => e!.Department)
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.ApprovedByUser)
                .Include(lr => lr.AttachedFile)
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime start, DateTime end, int? excludeId = null)
        {
            return await _context.LeaveRequests.AnyAsync(lr =>
                lr.EmployeeId == employeeId &&
                lr.Status != LeaveStatus.Cancelled &&
                lr.Status != LeaveStatus.Rejected &&
                (!excludeId.HasValue || lr.Id != excludeId.Value) &&
                lr.StartDate <= end && lr.EndDate >= start);
        }

        public async Task AddAsync(LeaveRequest request)
        {
            await _context.LeaveRequests.AddAsync(request);
        }

        public void Update(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<LeaveStatisticsDto> GetStatisticsAsync(int? employeeId = null)
        {
            var query = _context.LeaveRequests.AsQueryable();
            if (employeeId.HasValue)
            {
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            }

            var total = await query.CountAsync();
            var pending = await query.CountAsync(r => r.Status == LeaveStatus.Pending);
            var approved = await query.CountAsync(r => r.Status == LeaveStatus.Approved);
            var rejected = await query.CountAsync(r => r.Status == LeaveStatus.Rejected);
            var totalDaysApproved = await query
                .Where(r => r.Status == LeaveStatus.Approved)
                .SumAsync(r => r.TotalDays);

            List<DepartmentLeaveSummaryDto> summaries;

            if (employeeId.HasValue)
            {
                // Thống kê theo loại nghỉ phép của cá nhân nhân viên
                summaries = await query
                    .Include(r => r.LeaveType)
                    .GroupBy(r => r.LeaveType != null ? r.LeaveType.Name : "Khác")
                    .Select(g => new DepartmentLeaveSummaryDto
                    {
                        DepartmentName = g.Key,
                        TotalRequests = g.Count(),
                        ApprovedDays = g.Where(r => r.Status == LeaveStatus.Approved).Sum(r => r.TotalDays)
                    })
                    .ToListAsync();
            }
            else
            {
                // Thống kê theo phòng ban cho Quản lý / HR / Admin
                summaries = await query
                    .Include(r => r.Employee)
                    .ThenInclude(e => e!.Department)
                    .GroupBy(r => r.Employee != null && r.Employee.Department != null 
                        ? r.Employee.Department.Name 
                        : "Khác / Chưa phân phòng")
                    .Select(g => new DepartmentLeaveSummaryDto
                    {
                        DepartmentName = g.Key,
                        TotalRequests = g.Count(),
                        ApprovedDays = g.Where(r => r.Status == LeaveStatus.Approved).Sum(r => r.TotalDays)
                    })
                    .ToListAsync();
            }

            return new LeaveStatisticsDto
            {
                TotalRequests = total,
                PendingRequests = pending,
                ApprovedRequests = approved,
                RejectedRequests = rejected,
                TotalDaysOffApproved = totalDaysApproved,
                DepartmentSummaries = summaries
            };
        }
    }
}
