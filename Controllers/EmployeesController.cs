using DemoAPINET.DTOs.Employee;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Lấy danh sách toàn bộ nhân viên (Dành cho Admin, HR, Manager)
        [HttpGet]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        // Lấy thông tin nhân viên theo Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var emp = await _employeeService.GetByIdAsync(id);
            if (emp == null)
            {
                return NotFound(new { message = $"Không tìm thấy nhân viên với Id = {id}" });
            }

            return Ok(emp);
        }

        // Lấy hồ sơ nhân viên của tài khoản đang đăng nhập
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentEmployee()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
            {
                return NotFound(new { message = "Tài khoản hiện tại chưa được liên kết với hồ sơ nhân viên nào." });
            }

            return Ok(emp);
        }

        // Thêm mới nhân viên
        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
        {
            var created = await _employeeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Cập nhật thông tin nhân viên
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var success = await _employeeService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy nhân viên với Id = {id}" });
            }

            return Ok(new { message = "Cập nhật hồ sơ nhân viên thành công." });
        }

        // Xóa nhân viên
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy nhân viên với Id = {id}" });
            }

            return Ok(new { message = "Xóa nhân viên thành công." });
        }
    }
}
