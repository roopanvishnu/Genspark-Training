using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;

namespace TaskManagement.API.Implementations;

public class AuthService:IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _config = configuration;
    }
    
    
    public async Task<bool> RegisterAsync(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return false;

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDto?> GetCurrentUserAsync(HttpContext httpContext)
    {
        var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null;

        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    public async Task<(string accessToken, string refreshToken)?> LoginWithRefreshAsync(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var accessToken = GenerateJwtToken(user); // existing logic

        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken.Token);
    }


    public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.IsRevoked);

        if (token == null || token.ExpiresAt < DateTime.UtcNow)
            return null;

        // Optional: rotate the token (recommended)
        token.IsRevoked = true;

        var newRefresh = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = token.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        var accessToken = GenerateJwtToken(token.User);

        _context.RefreshTokens.Add(newRefresh);
        await _context.SaveChangesAsync();

        return (accessToken, newRefresh.Token);
    }
    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}