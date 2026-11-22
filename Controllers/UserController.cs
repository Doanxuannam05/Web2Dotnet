using DemoAPINET.DTOs;
using DemoAPINET.DTOs.Auth;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPINET.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    // GET: api/Users?pageNumber=1&pageSize=10&keyword=abc&role=Admin&sortBy=username&isDescending=false
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResponse<UserDto>>> GetAll([FromQuery] UserQueryParameters queryParams)
    {
        var result = await _userService.GetPagedAsync(queryParams);
        return Ok(result);
    }

    // GET: api/Users/5
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"Không tìm thấy người dùng có Id = {id}" });
        }

        return Ok(user);
    }

    // POST: api/Users/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto request)
    {
        var result = await _userService.RegisterAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    // PUT: api/Users/5
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] RegisterDto request)
    {
        var result = await _userService.UpdateAsync(id, request);
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    // DELETE: api/Users/5 (Chỉ dành cho Admin)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    // POST: api/Users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
        {
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
        }

        return Ok(new { Token = token });
    }
}
