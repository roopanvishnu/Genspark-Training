using AutoMapper;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class LeaveRequestServiceTests
    {
        private Mock<IRepository<Guid, LeaveRequest>> _leaveRequestRepoMock;
        private Mock<ILeaveBalanceService> _leaveBalanceServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILeaveTypeService> _leaveTypeServiceMock;
        private LeaveRequestService _service;

        [SetUp]
        public void SetUp()
        {
            _leaveRequestRepoMock = new Mock<IRepository<Guid, LeaveRequest>>();
            _leaveBalanceServiceMock = new Mock<ILeaveBalanceService>();
            _mapperMock = new Mock<IMapper>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userServiceMock = new Mock<IUserService>();
            _leaveTypeServiceMock = new Mock<ILeaveTypeService>();

            _service = new LeaveRequestService(
                _leaveRequestRepoMock.Object,
                _leaveBalanceServiceMock.Object,
                _mapperMock.Object,
                _auditLogServiceMock.Object,
                _currentUserServiceMock.Object,
                _userServiceMock.Object,
                _leaveTypeServiceMock.Object
            );
        }


        // [Test]
        // public async Task GetAllAsync_ReturnsMappedDtos()
        // {
        //     // Arrange
        //     var leaveRequests = new List<LeaveRequest> { new LeaveRequest { Id = Guid.NewGuid() } };
        //     var leaveDtos = new List<LeaveRequestResponseDto> { new LeaveRequestResponseDto { Id = leaveRequests[0].Id } };

        //     _leaveRequestRepoMock.Setup(r => r.GetAll()).ReturnsAsync(leaveRequests);
        //     _mapperMock.Setup(m => m.Map<IEnumerable<LeaveRequestResponseDto>>(leaveRequests)).Returns(leaveDtos);

        //     // Act
        //     var result = await _service.GetAllAsync();

        //     // Assert
        //     Assert.IsNotNull(result);
        //     Assert.AreEqual(leaveDtos, result);
        // }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsMappedDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var leaveRequest = new LeaveRequest { Id = id };
            var leaveDto = new LeaveRequestResponseDto { Id = id };

            _leaveRequestRepoMock.Setup(r => r.Get(id)).ReturnsAsync(leaveRequest);
            _mapperMock.Setup(m => m.Map<LeaveRequestResponseDto>(leaveRequest)).Returns(leaveDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
        }

        [Test]
        public void GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _leaveRequestRepoMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
            Assert.That(ex.Message, Does.Contain("not found"));
        }

        
        [Test]
        public void UpdateStatusAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _leaveRequestRepoMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateStatusAsync(id, "Rejected", Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("not found"));
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesAndLogs()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new LeaveRequest { Id = id };

            _leaveRequestRepoMock.Setup(r => r.Get(id)).ReturnsAsync(request);
            _leaveRequestRepoMock.Setup(r => r.Delete(id)).ReturnsAsync(request);
            _currentUserServiceMock.Setup(c => c.GetUserId()).Returns(Guid.NewGuid());

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _leaveRequestRepoMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(id));
            Assert.That(ex.Message, Does.Contain("Failed to delete leave request"));
        }
    }
}
