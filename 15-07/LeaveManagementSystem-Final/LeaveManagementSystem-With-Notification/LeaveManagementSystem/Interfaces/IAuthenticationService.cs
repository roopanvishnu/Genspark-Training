using LeaveManagementSystem.Models.DTOs;
using System.Security.Claims;

namespace LeaveManagementSystem.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string username);
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal user);
    }
}
