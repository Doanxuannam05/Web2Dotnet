using DemoAPINET.DTOs.Department;
using DemoAPINET.Models;
using DemoAPINET.Repositories;

namespace DemoAPINET.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DepartmentResponseDto>> GetAllAsync()
        {
            var depts = await _repository.GetAllAsync();
            return depts.Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                EmployeeCount = d.Employees?.Count ?? 0
            }).ToList();
        }

        public async Task<DepartmentResponseDto?> GetByIdAsync(int id)
        {
            var d = await _repository.GetByIdAsync(id);
            if (d == null) return null;

            return new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                EmployeeCount = d.Employees?.Count ?? 0
            };
        }

        public async Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto)
        {
            var dept = new Department
            {
                Name = dto.Name.Trim(),
                Code = dto.Code?.Trim().ToUpper(),
                Description = dto.Description?.Trim()
            };

            await _repository.AddAsync(dept);
            await _repository.SaveChangesAsync();

            return new DepartmentResponseDto
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code,
                Description = dept.Description,
                EmployeeCount = 0
            };
        }

        public async Task<bool> UpdateAsync(int id, DepartmentCreateDto dto)
        {
            var dept = await _repository.GetByIdAsync(id);
            if (dept == null) return false;

            dept.Name = dto.Name.Trim();
            dept.Code = dto.Code?.Trim().ToUpper();
            dept.Description = dto.Description?.Trim();

            _repository.Update(dept);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dept = await _repository.GetByIdAsync(id);
            if (dept == null) return false;

            _repository.Delete(dept);
            return await _repository.SaveChangesAsync();
        }
    }
}
