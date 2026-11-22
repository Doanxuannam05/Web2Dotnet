using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Department
{
    public class DepartmentCreateDto
    {
        [Required(ErrorMessage = "Tên phòng ban không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên phòng ban tối đa 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Mã phòng ban tối đa 20 ký tự.")]
        public string? Code { get; set; }

        [MaxLength(250, ErrorMessage = "Mô tả tối đa 250 ký tự.")]
        public string? Description { get; set; }
    }

    public class DepartmentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int EmployeeCount { get; set; }
    }
}
