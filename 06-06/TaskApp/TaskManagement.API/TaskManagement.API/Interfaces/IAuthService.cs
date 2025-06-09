using TaskManagement.API.DTOs;

namespace TaskManagement.API.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(UserRegisterDto dto);
    Task<string?> LoginAsync(UserLoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(HttpContext httpContext);
}