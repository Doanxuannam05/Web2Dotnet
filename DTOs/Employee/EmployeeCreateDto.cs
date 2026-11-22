using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; } = "Nhân viên";

        public int? DepartmentId { get; set; }

        public int AnnualLeaveQuota { get; set; } = 12;
    }
}
