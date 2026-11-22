using DemoAPINET.DTOs.Department;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var depts = await _departmentService.GetAllAsync();
            return Ok(depts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            if (dept == null)
            {
                return NotFound(new { message = $"Không tìm thấy phòng ban với Id = {id}" });
            }

            return Ok(dept);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateDto dto)
        {
            var created = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentCreateDto dto)
        {
            var success = await _departmentService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy phòng ban với Id = {id}" });
            }

            return Ok(new { message = "Cập nhật phòng ban thành công." });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _departmentService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy phòng ban với Id = {id}" });
            }

            return Ok(new { message = "Xóa phòng ban thành công." });
        }
    }
}
