using DemoAPINET.DTOs.Department;

namespace DemoAPINET.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDto>> GetAllAsync();
        Task<DepartmentResponseDto?> GetByIdAsync(int id);
        Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto);
        Task<bool> UpdateAsync(int id, DepartmentCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
