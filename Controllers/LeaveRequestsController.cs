using DemoAPINET.DTOs.Leave;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveRequestsController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        // Lấy danh sách tất cả đơn xin nghỉ có phân trang và bộ lọc (Dành cho Admin, HR, Manager)
        [HttpGet]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] LeaveQueryParameters query)
        {
            var result = await _leaveService.GetPagedRequestsAsync(query);
            return Ok(result);
        }

        // Nhân viên xem lịch sử các đơn xin nghỉ của chính mình
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var requests = await _leaveService.GetMyRequestsAsync(userId);
            return Ok(requests);
        }

        // Xem chi tiết một đơn xin nghỉ theo Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _leaveService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn xin nghỉ với Id = {id}" });
            }

            return Ok(request);
        }

        // Nhân viên nộp đơn xin nghỉ phép mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveRequestCreateDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var created = await _leaveService.CreateRequestAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Nhân viên hủy đơn xin nghỉ của mình (khi đơn còn ở trạng thái Pending)
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var (success, message) = await _leaveService.CancelRequestAsync(id, userId);
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // Quản lý / HR phê duyệt đơn xin nghỉ
        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var (success, message) = await _leaveService.ApproveRequestAsync(id, userId);
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // Quản lý / HR từ chối đơn xin nghỉ
        [HttpPut("{id:int}/reject")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Reject(int id, [FromBody] LeaveApprovalDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var (success, message) = await _leaveService.RejectRequestAsync(id, userId, dto.RejectionReason ?? string.Empty);
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // Thống kê báo cáo số ngày nghỉ (Tự động thích ứng: Nhân viên xem cá nhân, Quản lý/Admin xem toàn công ty)
        [HttpGet("statistics")]
        [Authorize]
        public async Task<IActionResult> GetStatistics()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            int.TryParse(userIdStr, out var userId);

            // Nếu người dùng là Nhân viên (Employee), trả về thống kê ngày nghỉ của chính nhân viên đó
            if (role == "Employee")
            {
                var personalStats = await _leaveService.GetStatisticsAsync(userId);
                return Ok(personalStats);
            }

            // Nếu là Admin, HR, Manager thì trả về thống kê toàn doanh nghiệp
            var companyStats = await _leaveService.GetStatisticsAsync(null);
            return Ok(companyStats);
        }
    }
}
