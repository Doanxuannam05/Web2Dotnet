using DemoAPINET.DTOs.Auth;

namespace DemoAPINET.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto);
}
