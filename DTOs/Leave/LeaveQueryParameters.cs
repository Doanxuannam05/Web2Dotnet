using DemoAPINET.Models;

namespace DemoAPINET.DTOs.Leave
{
    public class LeaveQueryParameters
    {
        private int _pageSize = 10;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(value, 1);
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Max(value, 1);
        }

        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public LeaveStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
