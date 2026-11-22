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
    }

    public class DepartmentLeaveSummaryDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int ApprovedDays { get; set; }
    }
}
