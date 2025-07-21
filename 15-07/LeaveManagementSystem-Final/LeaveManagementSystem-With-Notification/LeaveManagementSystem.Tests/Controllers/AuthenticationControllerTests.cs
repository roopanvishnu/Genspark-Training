using LeaveManagementSystem.Controllers;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Tests.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private AuthenticationController _controller;
        private Mock<IAuthenticationService> _mockAuthService;
        private ILogger<AuthenticationController> _logger;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _logger = new LoggerFactory().CreateLogger<AuthenticationController>();
            _controller = new AuthenticationController(_mockAuthService.Object, _logger);
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            var request = new UserLoginRequest { Email = "test@example.com", Password = "wrong" };
            _mockAuthService.Setup(s => s.LoginAsync(request)).ThrowsAsync(new UnauthorizedAccessException("Invalid login"));

            var result = await _controller.Login(request);
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task RefreshToken_ShouldReturnOk_WhenValidToken()
        {
            var request = new RefreshRequest { RefreshToken = "valid-refresh-token" };
            _mockAuthService.Setup(s => s.RefreshTokenAsync(request.RefreshToken)).ReturnsAsync("new-access-token");

            var result = await _controller.RefreshToken(request);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenSecurityTokenExceptionThrown()
        {
            var request = new RefreshRequest { RefreshToken = "invalid-token" };
            _mockAuthService.Setup(s => s.RefreshTokenAsync(request.RefreshToken)).ThrowsAsync(new Microsoft.IdentityModel.Tokens.SecurityTokenException("Invalid token"));

            var result = await _controller.RefreshToken(request);
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Logout_ShouldReturnOk_WhenSuccessful()
        {
            var email = "test@example.com";
            _mockAuthService.Setup(s => s.LogoutAsync(email)).Returns(Task.CompletedTask);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.Logout();
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            var result = await _controller.GetCurrentUser();
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }
    }
}
