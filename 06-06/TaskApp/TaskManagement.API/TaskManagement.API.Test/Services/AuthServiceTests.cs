/*
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs;
using TaskManagement.API.Implementations;
using TaskManagement.API.Models;
using Xunit;
using System;

public class AuthServiceTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly IConfiguration _config;

    public AuthServiceTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AuthTestDb")
            .Options;

        var inMemorySettings = new Dictionary<string, string> {
            {"JwtSettings:Secret", "my_super_secret_key_12345_67890_ABCDEF"},
            {"JwtSettings:Issuer", "TaskManagement.API"},
            {"JwtSettings:Audience", "TaskManagement.Client"},
        };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnTrue_WhenUserIsValid()
    {
        // Arrange
        await using var context = new AppDbContext(_options);
        var service = new AuthService(context, _config);

        var dto = new UserRegisterDto
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "1234",
            Role = "TeamMember"
        };

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result);
        Assert.NotNull(await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        await using var context = new AppDbContext(_options);
        
        // Seed user for login test
        var password = "securepassword";
        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            FullName = "Login User",
            Email = "login@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Manager"
        });
        await context.SaveChangesAsync();

        var service = new AuthService(context, _config);
        var dto = new UserLoginDto
        {
            Email = "login@example.com",
            Password = "securepassword"
        };

        // Act
        var token = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(token);
        Assert.IsType<string>(token);
        Assert.True(token.Length > 20); // Simple sanity check
    }
}

*/