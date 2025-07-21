using LeaveManagementSystem.Controllers;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Tests.Controllers
{
    [TestFixture]
    public class LeaveRequestControllerTests
    {
        private LeaveRequestController _controller;
        private Mock<ILeaveRequestService> _mockService;
        private ILogger<LeaveRequestController> _logger;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ILeaveRequestService>();
            _logger = new LoggerFactory().CreateLogger<LeaveRequestController>();
            _controller = new LeaveRequestController(_mockService.Object, _logger);
        }

        [Test]
        public async Task GetAll_ShouldReturnOk()
        {
            var mockData = new List<LeaveRequestResponseDto> { new LeaveRequestResponseDto { Reason = "Test Reason" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockData);

            var result = await _controller.GetAll();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetById_ShouldReturnOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var mockData = new LeaveRequestResponseDto { Reason = "Test" };
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(mockData);

            var result = await _controller.GetById(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetById(id);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var dto = new LeaveRequestDto { Reason = "Test" };
            var createdDto = new LeaveRequestResponseDto { Reason = "Test" };
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdDto);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task UpdateStatus_ShouldReturnOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateLeaveRequestStatusDto { Status = "Approved" };
            _mockService.Setup(s => s.UpdateStatusAsync(id, dto.Status, It.IsAny<Guid>())).ReturnsAsync(true);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.UpdateStatus(id, dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Delete_ShouldReturnOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            var result = await _controller.Delete(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.Delete(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
