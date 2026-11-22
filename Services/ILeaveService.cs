using DemoAPINET.DTOs;
using DemoAPINET.DTOs.Leave;
using DemoAPINET.DTOs.LeaveType;

namespace DemoAPINET.Services
{
    public interface ILeaveService
    {
        // Danh mục Loại nghỉ phép
        Task<List<LeaveTypeResponseDto>> GetAllLeaveTypesAsync();
        Task<LeaveTypeResponseDto?> GetLeaveTypeByIdAsync(int id);
        Task<LeaveTypeResponseDto> CreateLeaveTypeAsync(LeaveTypeCreateDto dto);
        Task<bool> UpdateLeaveTypeAsync(int id, LeaveTypeUpdateDto dto);
        Task<bool> DeleteLeaveTypeAsync(int id);

        // Đơn xin nghỉ phép
        Task<PagedResponse<LeaveRequestResponseDto>> GetPagedRequestsAsync(LeaveQueryParameters query);
        Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int userId);
        Task<LeaveRequestResponseDto?> GetRequestByIdAsync(int id);
        Task<LeaveRequestResponseDto> CreateRequestAsync(int userId, LeaveRequestCreateDto dto);
        Task<(bool Success, string Message)> CancelRequestAsync(int id, int userId);
        Task<(bool Success, string Message)> ApproveRequestAsync(int id, int approverUserId);
        Task<(bool Success, string Message)> RejectRequestAsync(int id, int approverUserId, string rejectionReason);
        Task<LeaveStatisticsDto> GetStatisticsAsync(int? userId = null);
    }
}
