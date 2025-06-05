using System;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticationService> _logger;
    public AuthenticationService(
                                    IRepository<string, User> userRepository,
                                    IEncryptionService encryptionService,
                                    ITokenService tokenService,
                                    ILogger<AuthenticationService> logger
                                )
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _tokenService = tokenService;
        _logger = logger;
    }
    public async Task<UserLoginResponse> Login(UserLoginRequest req)
    {
        try
        {
            User dbUser = await _userRepository.Get(req.Username);
            if (dbUser == null) {
                _logger.LogCritical("User Not found");
                throw new Exception("User not found");
            }
            EncryptModel encryptedPassword = _encryptionService.EncryptData(new EncryptModel
            {
                Data = req.Password,
                HashKey = dbUser.HashKey
            });
            for (int i = 0; i < dbUser.Password!.Length; i++)
            {
                if (dbUser.Password[i] != encryptedPassword.EncryptedData![i])
                {
                    _logger.LogError("Invalid Password");
                    throw new Exception("Invalid Password");
                }
            }
            string token = await _tokenService.GenerateToken(dbUser);
            return new UserLoginResponse
            {
                Username = dbUser.Username,
                Token = token
            };
        }
        catch
        {
            throw;
        }
    }
    public async Task<GoogleLoginDTO> LoginWithGoogle(GoogleLoginDTO req)
    {
        try
        {
            User dbUser = await _userRepository.Get(req.Email!);
            if (dbUser == null) {
                _logger.LogCritical("User Not found");
                throw new Exception("User not found");
            }
            string token = await _tokenService.GenerateToken(dbUser);
            req.Token = token;
            return req;
        }
        catch
        {
            throw;
        }
    }
}
