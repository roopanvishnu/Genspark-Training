using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace LeaveManagementSystem.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IRepository<Guid, User> userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            var user = (await _userRepository.GetAll()).FirstOrDefault(u =>
                u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenString = _tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Configurable
                IsRevoked = false
            };

            await _refreshTokenRepository.Add(refreshToken);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            return new UserLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByToken(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                throw new SecurityTokenException("Invalid or expired refresh token");
            }

            // Revoke old token
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.Update(storedToken.Id, storedToken);

            var user = storedToken.User ?? (await _userRepository.Get(storedToken.UserId));

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshTokenString = _tokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.Add(newRefreshToken);

            _logger.LogInformation("Refresh token used to generate new access token for {Email}", user.Email);

            return newAccessToken;
        }

        public async Task LogoutAsync(string email)
        {
            var user = (await _userRepository.GetAll()).FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("Logout attempted for unknown user {Email}", email);
                return;
            }

            var tokens = await _refreshTokenRepository.GetTokensByUserId(user.Id);
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                await _refreshTokenRepository.Update(token.Id, token);
            }

            _logger.LogInformation("User {Email} logged out and refresh tokens revoked", email);
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var email = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = (await _userRepository.GetAll()).FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("GetCurrentUser failed: User not found with email {Email}", email);
                throw new UnauthorizedAccessException("User not found");
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
