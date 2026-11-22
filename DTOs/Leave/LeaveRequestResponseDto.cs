using DemoAPINET.Models;

namespace DemoAPINET.DTOs.Leave
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; }
        public string StatusName => Status switch
        {
            LeaveStatus.Pending => "Chờ duyệt",
            LeaveStatus.Approved => "Đã duyệt",
            LeaveStatus.Rejected => "Từ chối",
            LeaveStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
        public string? RejectionReason { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public int? AttachedFileId { get; set; }
        public string? AttachedFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
