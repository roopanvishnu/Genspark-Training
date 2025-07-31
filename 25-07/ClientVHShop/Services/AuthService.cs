using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ChienVHShopOnline.Services;

public class AuthService
{
    private UserRepo _userRepo;
    private SymmetricSecurityKey securityKey;
    public AuthService(UserRepo userRepo, IConfiguration configuration)
    {
        _userRepo = userRepo;
        securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSecret"]!));
    }
    public async Task<LoginResponseDTO> Login(UserDTO creds)
    {
        User? user = (await _userRepo.GetAll()).FirstOrDefault(u => u.Username == creds.Username);
        if (user == null)
        {
            throw new Exception("No such user found");
        }
        if (BCrypt.Net.BCrypt.EnhancedVerify(creds.Password, user.Password))
        {
            return new LoginResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                AccessToken = GenerateToken(user.UserId)
            };
        }
        else
        {
            throw new Exception("Password is Invalid");
        }
    }

    public string GenerateToken(int userId)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = DateTime.Now.AddDays(1)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}