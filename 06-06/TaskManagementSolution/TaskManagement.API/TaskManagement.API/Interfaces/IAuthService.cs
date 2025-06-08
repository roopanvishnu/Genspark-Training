using TaskManagement.API.DTOs.Auth;
using TaskManagement.API.Models;

namespace TaskManagement.API.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
}