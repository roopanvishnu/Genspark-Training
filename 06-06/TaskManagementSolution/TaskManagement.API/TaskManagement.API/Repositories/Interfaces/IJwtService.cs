using System.Security.Claims;
namespace TaskManagement.API.Repositories.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(Guid userId, string email, string role);
    Task<string> GenerateRefreshTokenAsync();
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid userId);
    Task RevokeRefreshTokenAsync(Guid userId);
    Task<string> GetUserIdFromTokenAsync(string token);
}