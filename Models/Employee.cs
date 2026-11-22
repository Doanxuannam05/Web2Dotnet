using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; } = "Nhân viên";

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        // Hạn mức ngày nghỉ phép năm (mặc định 12 ngày)
        public int AnnualLeaveQuota { get; set; } = 12;

        // Số ngày nghỉ phép còn lại trong năm
        public int RemainingLeaveDays { get; set; } = 12;

        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
