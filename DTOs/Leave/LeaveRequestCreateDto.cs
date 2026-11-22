using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Leave
{
    public class LeaveRequestCreateDto
    {
        public int? EmployeeId { get; set; } // Nếu để trống, hệ thống tự động gán theo tài khoản nhân viên đang đăng nhập

        [Required(ErrorMessage = "Vui lòng chọn loại nghỉ phép.")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do nghỉ.")]
        [MaxLength(500, ErrorMessage = "Lý do nghỉ tối đa 500 ký tự.")]
        public string Reason { get; set; } = string.Empty;

        // Mã file minh chứng đính kèm (nếu có, upload qua /api/filesdb/upload)
        public int? AttachedFileId { get; set; }
    }
}
