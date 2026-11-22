namespace DemoAPINET.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Position { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime HireDate { get; set; }
        public int AnnualLeaveQuota { get; set; }
        public int RemainingLeaveDays { get; set; }
    }
}
