using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs.Auth;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagement.API.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
        {
            throw new Exception("User already exists");
        }

        var passwordHash = HashPassword(dto.Password);
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role,
            Password = dto.Password,
            PasswordHash = passwordHash,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken(),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
        };
    }


public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid credentials");
        }

        return new AuthResponseDto()
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken(),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
        };
    }

    public string GenerateAccessToken(AppUser user)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var expiration = DateTime.UtcNow.AddDays(1);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, audience, claims, expires: expiration, signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private byte[] HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password) switch
        {
            string hashed => Encoding.UTF8.GetBytes(hashed),
            _ => throw new Exception("Password doesn't match"),
        };
    }

    private bool VerifyPassword(string password, byte[] storedHash)
    {
        var hashString = Encoding.UTF8.GetString(storedHash);
        return BCrypt.Net.BCrypt.Verify(password, hashString);
    }

}
