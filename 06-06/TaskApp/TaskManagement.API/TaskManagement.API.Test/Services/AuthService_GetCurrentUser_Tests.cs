using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs;
using TaskManagement.API.Implementations;
using TaskManagement.API.Models;
using Xunit;
using System;

public class AuthService_GetCurrentUser_Tests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly IConfiguration _config;

    public AuthService_GetCurrentUser_Tests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetCurrentUserTestDb")
            .Options;

        var inMemorySettings = new Dictionary<string, string> {
            {"JwtSettings:Secret", "my_super_secret_key_12345678901234567890"}, // 32+ chars
            {"JwtSettings:Issuer", "TaskManagement.API"},
            {"JwtSettings:Audience", "TaskManagement.Client"},
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUser_WhenValidClaimsExist()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "Mock User",
            Email = "mock@example.com",
            PasswordHash = "dummy",
            Role = "Manager"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockContext = new DefaultHttpContext { User = claimsPrincipal };

        var service = new AuthService(context, _config);

        // Act
        var result = await service.GetCurrentUserAsync(mockContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("mock@example.com", result.Email);
        Assert.Equal("Mock User", result.FullName);
        Assert.Equal("Manager", result.Role);
    }
}
