using LeaveManagementSystem.Controllers;
using LeaveManagementSystem.Interfaces;
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
    public class LeaveBalanceControllerTests
    {
        private LeaveBalanceController _controller;
        private Mock<ILeaveBalanceService> _leaveBalanceServiceMock;
        private Mock<ILogger<LeaveBalanceController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _leaveBalanceServiceMock = new Mock<ILeaveBalanceService>();
            _loggerMock = new Mock<ILogger<LeaveBalanceController>>();
            _controller = new LeaveBalanceController(_leaveBalanceServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetLeaveBalancesForUser_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var leaveBalanceDto = new UserLeaveBalanceResponseDto();
            _leaveBalanceServiceMock.Setup(s => s.GetLeaveBalancesForUserAsync(userId))
                .ReturnsAsync(leaveBalanceDto);

            var result = await _controller.GetLeaveBalancesForUser(userId);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<UserLeaveBalanceResponseDto>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }

        [Test]
        public async Task GetLeaveBalanceForUserByType_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            var leaveBalanceDto = new UserLeaveBalanceForTypeResponseDto();
            _leaveBalanceServiceMock.Setup(s => s.GetLeaveBalanceForTypeAsync(userId, leaveTypeId))
                .ReturnsAsync(leaveBalanceDto);

            var result = await _controller.GetLeaveBalanceForUserByType(userId, leaveTypeId);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<UserLeaveBalanceForTypeResponseDto>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }

        [Test]
        public async Task InitializeLeaveBalancesForUser_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            _leaveBalanceServiceMock.Setup(s => s.InitializeLeaveBalancesForUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.InitializeLeaveBalancesForUser(userId);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }

        [Test]
        public async Task InitializeLeaveBalanceForNewLeaveType_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            int standardLeaveCount = 10;

            _leaveBalanceServiceMock.Setup(s => s.InitializeLeaveBalanceForNewLeaveTypeAsync(userId, leaveTypeId, standardLeaveCount))
                .Returns(Task.CompletedTask);

            var result = await _controller.InitializeLeaveBalanceForNewLeaveType(userId, leaveTypeId, standardLeaveCount);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }

        [Test]
        public async Task DeductLeaveBalance_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            int days = 2;

            _leaveBalanceServiceMock.Setup(s => s.DeductLeaveBalanceAsync(userId, leaveTypeId, days))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeductLeaveBalance(userId, leaveTypeId, days);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }

        [Test]
        public async Task ResetLeaveBalancesForUser_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            _leaveBalanceServiceMock.Setup(s => s.ResetLeaveBalancesForUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.ResetLeaveBalancesForUser(userId);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.IsNotNull(apiResponse.Data);
        }
    }
}
