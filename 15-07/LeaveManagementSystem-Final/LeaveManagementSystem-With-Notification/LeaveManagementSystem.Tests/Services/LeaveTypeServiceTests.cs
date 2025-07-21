using AutoMapper;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class LeaveTypeServiceTests
    {
        private Mock<IRepository<Guid, LeaveType>> _repositoryMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IMapper> _mapperMock;
        private LeaveTypeService _leaveTypeService;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Guid, LeaveType>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mapperMock = new Mock<IMapper>();

            _leaveTypeService = new LeaveTypeService(
                _repositoryMock.Object,
                _auditLogServiceMock.Object,
                _currentUserServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsLeaveType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var leaveType = new LeaveType { Id = id };
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync(leaveType);

            // Act
            var result = await _leaveTypeService.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(id, result.Id);
        }

        [Test]
        public void GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveType)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _leaveTypeService.GetByIdAsync(id));
        }

        [Test]
        public async Task CreateAsync_ValidDto_ReturnsCreatedLeaveType()
        {
            // Arrange
            var dto = new LeaveTypeDto { Name = "Sick Leave", Description = "Sick leave description" };
            var leaveType = new LeaveType { Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description };
            
            _mapperMock.Setup(m => m.Map<LeaveType>(dto)).Returns(leaveType);
            _repositoryMock.Setup(r => r.Add(It.IsAny<LeaveType>())).ReturnsAsync(leaveType);

            // Act
            var result = await _leaveTypeService.CreateAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.Name);

        }

        [Test]
        public async Task UpdateAsync_ValidId_UpdatesAndReturnsLeaveType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new LeaveType { Id = id, Name = "Old Name" };
            var dto = new LeaveTypeDto { Name = "New Name", Description = "New Desc" };
            
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync(existing);
            _repositoryMock.Setup(r => r.Update(id, existing)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(dto, existing));

            // Act
            var result = await _leaveTypeService.UpdateAsync(id, dto);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdateAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new LeaveTypeDto();
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveType)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _leaveTypeService.UpdateAsync(id, dto));
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesLeaveType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new LeaveType { Id = id };
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync(existing);
            _repositoryMock.Setup(r => r.Delete(id)).ReturnsAsync(existing);

            // Act
            var result = await _leaveTypeService.DeleteAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Get(id)).ReturnsAsync((LeaveType)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _leaveTypeService.DeleteAsync(id));
        }
    }
}