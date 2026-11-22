namespace DemoAPINET.DTOs;

public class UserQueryParameters
{
    private int _pageSize = 10;
    private int _pageNumber = 1;

    // Số trang: chỉ cần >= 1
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(value, 1);
    }

    // Nhập bao nhiêu nhận bấy nhiêu (chỉ chặn nếu nhập <= 0 thì tự về 1)
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Max(value, 1);
    }

    public string? Keyword { get; set; }
    public string? Role { get; set; }
    public string? SortBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
} 