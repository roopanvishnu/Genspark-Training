/*
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs.TaskDtos;
using TaskManagement.API.Hubs;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;
using TaskManagement.API.Services.Implementations;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<AppDbContext> mockContext;
    private readonly IMapper mapper;
    private readonly IWebHostEnvironment env;
    private readonly Mock<IHubContext<TaskHub>> mockHubContext;

    public TaskServiceTests()
    {
        // Setup AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee.FullName))
                .ForMember(dest => dest.AttachmentFileName, opt => opt.MapFrom(src => src.Attachments.FirstOrDefault().FileName));
        });
        mapper = config.CreateMapper();

        // Setup WebHostEnvironment
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(m => m.WebRootPath).Returns(Directory.GetCurrentDirectory());
        env = mockEnv.Object;

        // Setup HubContext
        mockHubContext = new Mock<IHubContext<TaskHub>>();

        // Setup DbContext with in-memory DB
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        mockContext = new Mock<AppDbContext>(options);

        using var context = new AppDbContext(options);
        // Seed a valid user (required fields must be set)
        context.Users.Add(new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FullName = "Test User",
            Email = "test@example.com",  // Required
            PasswordHash = "hashedpassword", // Required
            Role = "TeamMember",
            IsDeleted = false
        });
        context.SaveChanges();

    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateAndReturnTaskDto_WhenValidInput()
    {
        // Arrange
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var dto = new CreateTaskDto
        {
            Title = "Test Task",
            Description = "Description",
            AssigneeId = userId,
            Attachment = null
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("CreateTaskDb")
            .Options;

        await using var context = new AppDbContext(options);

        context.Users.Add(new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "testuser@example.com",               // ✅ Required
            PasswordHash = "hashedpassword123",           // ✅ Required
            Role = "TeamMember",
            IsDeleted = false
        });

        await context.SaveChangesAsync();

        var service = new TaskService(context, mapper, env, mockHubContext.Object);

        // Act
        var result = await service.CreateTaskAsync(dto, "adminUserId");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(userId, result.AssigneeId);
    }
}
*/