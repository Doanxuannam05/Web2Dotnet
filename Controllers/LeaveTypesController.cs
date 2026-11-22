using DemoAPINET.DTOs.LeaveType;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveTypesController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _leaveService.GetAllLeaveTypesAsync();
            return Ok(types);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var type = await _leaveService.GetLeaveTypeByIdAsync(id);
            if (type == null)
            {
                return NotFound(new { message = $"Không tìm thấy loại nghỉ phép với Id = {id}" });
            }

            return Ok(type);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create([FromBody] LeaveTypeCreateDto dto)
        {
            var created = await _leaveService.CreateLeaveTypeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Update(int id, [FromBody] LeaveTypeUpdateDto dto)
        {
            var success = await _leaveService.UpdateLeaveTypeAsync(id, dto);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy loại nghỉ phép với Id = {id}" });
            }

            return Ok(new { message = "Cập nhật loại nghỉ phép thành công." });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _leaveService.DeleteLeaveTypeAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy loại nghỉ phép với Id = {id}" });
            }

            return Ok(new { message = "Xóa loại nghỉ phép thành công." });
        }
    }
}
