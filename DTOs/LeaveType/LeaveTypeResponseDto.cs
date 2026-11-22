namespace DemoAPINET.DTOs.LeaveType
{
    public class LeaveTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }
    }
}
