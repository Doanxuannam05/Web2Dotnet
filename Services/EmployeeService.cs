using DemoAPINET.DTOs.Employee;
using DemoAPINET.Models;
using DemoAPINET.Repositories;

namespace DemoAPINET.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await _repository.GetAllAsync();
            return employees.Select(MapToResponseDto).ToList();
        }

        public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
        {
            var emp = await _repository.GetByIdAsync(id);
            return emp == null ? null : MapToResponseDto(emp);
        }

        public async Task<EmployeeResponseDto?> GetByUserIdAsync(int userId)
        {
            var emp = await _repository.GetByUserIdAsync(userId);
            return emp == null ? null : MapToResponseDto(emp);
        }

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                UserId = dto.UserId,
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                PhoneNumber = dto.PhoneNumber?.Trim(),
                Position = dto.Position?.Trim() ?? "Nhân viên",
                DepartmentId = dto.DepartmentId,
                HireDate = DateTime.UtcNow,
                AnnualLeaveQuota = dto.AnnualLeaveQuota > 0 ? dto.AnnualLeaveQuota : 12,
                RemainingLeaveDays = dto.AnnualLeaveQuota > 0 ? dto.AnnualLeaveQuota : 12
            };

            await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(employee.Id);
            return MapToResponseDto(created ?? employee);
        }

        public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var emp = await _repository.GetByIdAsync(id);
            if (emp == null) return false;

            emp.FullName = dto.FullName.Trim();
            emp.PhoneNumber = dto.PhoneNumber?.Trim();
            emp.Position = dto.Position?.Trim();
            emp.DepartmentId = dto.DepartmentId;
            emp.AnnualLeaveQuota = dto.AnnualLeaveQuota;
            emp.RemainingLeaveDays = dto.RemainingLeaveDays;

            _repository.Update(emp);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emp = await _repository.GetByIdAsync(id);
            if (emp == null) return false;

            _repository.Delete(emp);
            return await _repository.SaveChangesAsync();
        }

        private static EmployeeResponseDto MapToResponseDto(Employee e)
        {
            return new EmployeeResponseDto
            {
                Id = e.Id,
                UserId = e.UserId,
                UserName = e.User?.UserName,
                FullName = e.FullName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Position = e.Position,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department?.Name,
                HireDate = e.HireDate,
                AnnualLeaveQuota = e.AnnualLeaveQuota,
                RemainingLeaveDays = e.RemainingLeaveDays
            };
        }
    }
}
