using TaskManagement.API.DTOs;

namespace TaskManagement.API.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(UserRegisterDto dto);
    Task<string?> LoginAsync(UserLoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(HttpContext httpContext);
    
    // Refresh Token Logic
    Task<(string accessToken, string refreshToken)?> LoginWithRefreshAsync(UserLoginDto dto);
    Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);

}