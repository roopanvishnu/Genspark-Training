using LeaveManagementSystem.Controllers;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Tests.Controllers
{
    [TestFixture]
    public class LeaveTypeControllerTests
    {
        private LeaveTypeController _controller;
        private Mock<ILeaveTypeService> _mockService;
        private ILogger<LeaveTypeController> _logger;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ILeaveTypeService>();
            _logger = new LoggerFactory().CreateLogger<LeaveTypeController>();
            _controller = new LeaveTypeController(_mockService.Object, _logger);
        }

        [Test]
        public async Task GetAll_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var expected = new List<LeaveTypeResponseDto> { new LeaveTypeResponseDto { Id = Guid.NewGuid(), Name = "Sick Leave" } };
            _mockService.Setup(s => s.GetAllAsync(1, 10, null, null, "asc")).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<ApiResponse<IEnumerable<LeaveTypeResponseDto>>>(okResult.Value);
        }

        [Test]
        public async Task GetById_ShouldReturnOkResult_WhenLeaveTypeExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var leaveType = new LeaveType { Id = id, Name = "Casual Leave" };
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(leaveType);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<LeaveType>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(leaveType.Id, apiResponse.Data.Id);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenLeaveTypeDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtActionResult_WhenValidRequest()
        {
            // Arrange
            var dto = new LeaveTypeDto { Name = "Annual Leave" };
            var leaveType = new LeaveType { Id = Guid.NewGuid(), Name = "Annual Leave" };
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(leaveType);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAtResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual("GetById", createdAtResult.ActionName);
        }

        [Test]
        public async Task Update_ShouldReturnOkResult_WhenLeaveTypeUpdated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new LeaveTypeDto { Name = "Updated Leave" };
            var leaveType = new LeaveType { Id = id, Name = "Updated Leave" };
            _mockService.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync(leaveType);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<LeaveType>;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(leaveType.Name, apiResponse.Data.Name);
        }

        [Test]
        public async Task Update_ShouldReturnNotFound_WhenLeaveTypeNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new LeaveTypeDto { Name = "Invalid Leave" };
            _mockService.Setup(s => s.UpdateAsync(id, dto)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Delete_ShouldReturnOkResult_WhenLeaveTypeDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<LeaveType>;
            Assert.IsTrue(apiResponse.Success);
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenLeaveTypeDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
