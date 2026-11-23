namespace DemoAPINET.DTOs.Leave
{
    public class LeaveStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalDaysOffApproved { get; set; }
        public List<DepartmentLeaveSummaryDto> DepartmentSummaries { get; set; } = new();
        public List<EmployeeLeaveSummaryDto> EmployeeSummaries { get; set; } = new();
    }

    public class DepartmentLeaveSummaryDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int ApprovedDays { get; set; }
    }

    public class EmployeeLeaveSummaryDto
    {
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin", "HR", "Employee"
        public string Position { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int AnnualLeaveQuota { get; set; } = 12;
        public int RemainingLeaveDays { get; set; } = 12;
        public int TotalDaysRequested { get; set; } // Tổng ngày đã xin nghỉ
        public int TotalDaysApproved { get; set; }  // Tổng ngày đã được duyệt
        public int TotalRequests { get; set; }      // Tổng số đơn
        public int PendingRequests { get; set; }    // Số đơn đang chờ duyệt
    }
}
