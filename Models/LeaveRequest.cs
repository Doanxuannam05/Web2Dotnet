using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.Models
{
    public enum LeaveStatus
    {
        Pending = 0,   // Chờ duyệt
        Approved = 1,  // Đã duyệt
        Rejected = 2,  // Từ chối
        Cancelled = 3  // Đã hủy bởi nhân viên
    }

    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Tổng số ngày nghỉ (tính theo ngày làm việc hoặc số ngày)
        public int TotalDays { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        // Người phê duyệt / từ chối
        public int? ApprovedByUserId { get; set; }
        public User? ApprovedByUser { get; set; }

        // File minh chứng đính kèm (giấy khám bệnh, quyết định...)
        public int? AttachedFileId { get; set; }
        public StoredFile? AttachedFile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
