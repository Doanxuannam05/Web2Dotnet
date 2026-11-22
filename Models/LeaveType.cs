using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.Models
{
    public class LeaveType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Nghỉ phép năm, Nghỉ ốm, Thai sản, Nghỉ không lương...

        public int DefaultDays { get; set; } = 12;

        public bool IsPaid { get; set; } = true;

        [MaxLength(250)]
        public string? Description { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
