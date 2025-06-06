using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Repositories.Interfaces;

namespace TaskManagement.API.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<Guid, string> _refreshTokens = new();

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GenerateAccessTokenAsync(Guid userId, string email, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "secret"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public Task<string> GenerateRefreshTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");
        return Task.FromResult(token);
    }

    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "secret");
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);
            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch
        {
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }

    public Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid userId)
    {
        return Task.FromResult(_refreshTokens.TryGetValue(userId, out var stored) && stored == refreshToken);
    }

    public Task RevokeRefreshTokenAsync(Guid userId)
    {
        _refreshTokens.Remove(userId);
        return Task.CompletedTask;
    }

    public Task<string> GetUserIdFromTokenAsync(string token)
    {
        var principal = ValidateTokenAsync(token).Result;
        var userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        return Task.FromResult(userId);
    }
}
