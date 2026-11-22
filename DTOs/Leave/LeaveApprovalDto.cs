using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Leave
{
    public class LeaveApprovalDto
    {
        public bool IsApproved { get; set; } = true;

        [MaxLength(500, ErrorMessage = "Lý do từ chối tối đa 500 ký tự.")]
        public string? RejectionReason { get; set; }
    }
}
