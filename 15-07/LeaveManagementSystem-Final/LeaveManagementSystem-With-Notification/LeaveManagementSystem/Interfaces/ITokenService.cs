using LeaveManagementSystem.Models;
using System.Security.Claims;

namespace LeaveManagementSystem.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
