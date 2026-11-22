using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.LeaveType
{
    public class LeaveTypeCreateDto
    {
        [Required(ErrorMessage = "Tên loại nghỉ phép không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên loại nghỉ phép tối đa 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 365, ErrorMessage = "Số ngày mặc định phải từ 1 đến 365.")]
        public int DefaultDays { get; set; } = 12;

        public bool IsPaid { get; set; } = true;

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
