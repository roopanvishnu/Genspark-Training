using System;
using Notify.Models;
using Notify.Models.DTOs;

namespace Notify.Services;

public class AuthenticationService
{
    private readonly UserService _userService;
    private readonly HashingService _hashingService;
    private readonly TokenService _tokenService;
    public AuthenticationService(UserService userService, HashingService hashingService, TokenService tokenService)
    {
        _userService = userService;
        _hashingService = hashingService;
        _tokenService = tokenService;
    }
    public async Task<LoginResponseDTO> Login(LoginRequestDTO dto)
    {
        User user = await _userService.GetUserByEmail(dto.Email);
        if (user == null) throw new Exception("No user found");
        var hashedData = new HashDTO { Data = dto.Password, HashKey = user.HashKey };
        hashedData = _hashingService.HashData(hashedData);
        if (user.Password!.ToString()!.Equals(hashedData.HashedData!.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            string Token = _tokenService.GenerateToken(user.Email, user.Role);
            LoginResponseDTO res = new LoginResponseDTO
            {
                Email = user.Email,
                Role = user.Role,
                Token = Token
            };
            return res;
        }
        else
        {
            throw new Exception("Invalid Password");
        }
    }
}
