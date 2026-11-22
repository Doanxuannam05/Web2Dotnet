using DemoAPINET.DTOs.Auth;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var registrationResult = await _authService.RegisterAsync(dto);

            if (!registrationResult.Success)
            {
                return BadRequest(new { message = registrationResult.Message });
            }

            return Ok(new { message = registrationResult.Message });
        }
    }
}
