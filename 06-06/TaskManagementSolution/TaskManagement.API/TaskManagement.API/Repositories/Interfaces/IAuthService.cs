using TaskManagement.API.DTOs.Request.Auth;
using TaskManagement.API.DTOs.Response.Auth;
using TaskManagement.API.DTOs.Response.Common;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponseDto<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponseDto<string>> LogoutAsync(string userId);
    Task<ApiResponseDto<UserProfileResponseDto>> GetCurrentUserAsync(string userId);
    Task<ApiResponseDto<UserProfileResponseDto>> RegisterAsync(RegisterRequestDto request);
}