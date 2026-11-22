using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        public int? DepartmentId { get; set; }

        public int AnnualLeaveQuota { get; set; } = 12;

        public int RemainingLeaveDays { get; set; } = 12;
    }
}
