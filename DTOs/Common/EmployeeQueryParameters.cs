namespace DemoAPINET.DTOs
{
    public class EmployeeQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private int _pageNumber = 1;

        // Phân trang
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(value, 1);
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        // Tìm kiếm (Searching) theo từ khóa chuỗi
        public string? Keyword { get; set; }

        // Lọc (Filtering) chính xác theo thuộc tính
        public int? DepartmentId { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }

        // Sắp xếp (Sorting)
        public string? SortBy { get; set; } = "Id"; // Mặc định sắp xếp theo Id
        public bool IsDescending { get; set; } = false;
    }
}