using NUnit.Framework;
using Moq;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IRepository<Guid, User>> _userRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<AuthenticationService>> _loggerMock;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IRepository<Guid, User>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthenticationService>>();

            _authService = new AuthenticationService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsUserLoginResponse()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") };
            var request = new UserLoginRequest { Email = "test@example.com", Password = "password123" };

            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });
            _tokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("access_token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh_token");
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = user.Email });

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("access_token", result.AccessToken);
            Assert.AreEqual("refresh_token", result.RefreshToken);
            Assert.AreEqual("test@example.com", result.User.Email);
        }

        [Test]
        public void LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") };
            var request = new UserLoginRequest { Email = "test@example.com", Password = "wrongpassword" };

            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(request));
            Assert.That(ex.Message, Is.EqualTo("Invalid credentials"));
        }

        [Test]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewAccessToken()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });
            _tokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("new_access_token");

            // Simulate login to set refresh token
            var privateField = typeof(AuthenticationService)
                .GetField("_refreshTokens", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var dict = (System.Collections.Concurrent.ConcurrentDictionary<string, string>)privateField.GetValue(null);
            dict["test@example.com"] = "valid_refresh_token";

            // Act
            var result = await _authService.RefreshTokenAsync("valid_refresh_token");

            // Assert
            Assert.AreEqual("new_access_token", result);
        }

        [Test]
        public void RefreshTokenAsync_InvalidToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var invalidToken = "invalid_refresh_token";

            // Act & Assert
            var ex = Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshTokenAsync(invalidToken));
            Assert.That(ex.Message, Is.EqualTo("Invalid refresh token"));
        }

        [Test]
        public async Task LogoutAsync_RemovesRefreshToken()
        {
            // Arrange
            var privateField = typeof(AuthenticationService)
                .GetField("_refreshTokens", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var dict = (System.Collections.Concurrent.ConcurrentDictionary<string, string>)privateField.GetValue(null);
            dict["test@example.com"] = "some_token";

            // Act
            await _authService.LogoutAsync("test@example.com");

            // Assert
            Assert.IsFalse(dict.ContainsKey("test@example.com"));
        }

        [Test]
        public async Task GetCurrentUserAsync_ValidUser_ReturnsUserDto()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "test@example.com")
            }));

            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = user.Email });

            // Act
            var result = await _authService.GetCurrentUserAsync(claims);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public void GetCurrentUserAsync_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "unknown@example.com")
            }));

            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.GetCurrentUserAsync(claims));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }
    }
}
