using NUnit.Framework;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Services;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class LeaveBalanceServiceTests
    {
        private Mock<IRepository<Guid, LeaveBalance>> _leaveBalanceRepoMock;
        private Mock<IRepository<Guid, LeaveType>> _leaveTypeRepoMock;
        private Mock<IRepository<Guid, User>> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private LeaveBalanceService _leaveBalanceService;

        [SetUp]
        public void Setup()
        {
            _leaveBalanceRepoMock = new Mock<IRepository<Guid, LeaveBalance>>();
            _leaveTypeRepoMock = new Mock<IRepository<Guid, LeaveType>>();
            _userRepoMock = new Mock<IRepository<Guid, User>>();
            _mapperMock = new Mock<IMapper>();

            _leaveBalanceService = new LeaveBalanceService(
                _leaveBalanceRepoMock.Object,
                _leaveTypeRepoMock.Object,
                _userRepoMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetLeaveBalancesForUserAsync_ValidUser_ReturnsLeaveBalances()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "TestUser",
                LeaveBalances = new List<LeaveBalance>
                {
                    new LeaveBalance { LeaveTypeId = Guid.NewGuid(), TotalLeaves = 10, UsedLeaves = 2 }
                }
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<List<LeaveBalanceResponseDto>>(It.IsAny<List<LeaveBalance>>()))
                .Returns(new List<LeaveBalanceResponseDto>());

            // Act
            var result = await _leaveBalanceService.GetLeaveBalancesForUserAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual("TestUser", result.UserName);
        }

        [Test]
        public void GetLeaveBalancesForUserAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _leaveBalanceService.GetLeaveBalancesForUserAsync(userId));
            Assert.That(ex.Message, Does.Contain("User with ID"));
        }

        [Test]
        public async Task GetLeaveBalanceForTypeAsync_Valid_ReturnsLeaveBalance()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "TestUser",
                LeaveBalances = new List<LeaveBalance>
                {
                    new LeaveBalance { LeaveTypeId = leaveTypeId, TotalLeaves = 10, UsedLeaves = 5 }
                }
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<LeaveBalanceResponseDto>(It.IsAny<LeaveBalance>()))
                .Returns(new LeaveBalanceResponseDto());

            // Act
            var result = await _leaveBalanceService.GetLeaveBalanceForTypeAsync(userId, leaveTypeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }

        [Test]
        public void GetLeaveBalanceForTypeAsync_LeaveBalanceNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                LeaveBalances = new List<LeaveBalance>()
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _leaveBalanceService.GetLeaveBalanceForTypeAsync(userId, leaveTypeId));
            Assert.That(ex.Message, Does.Contain("Leave Balance for LeaveType"));
        }

        [Test]
        public async Task InitializeLeaveBalancesForUserAsync_UserWithoutBalances_AddsBalances()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _leaveBalanceRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LeaveBalance>());
            _leaveTypeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LeaveType>
            {
                new LeaveType { Id = Guid.NewGuid(), StandardLeaveCount = 12 }
            });

            // Act
            await _leaveBalanceService.InitializeLeaveBalancesForUserAsync(userId);

            // Assert
            _leaveBalanceRepoMock.Verify(r => r.Add(It.IsAny<LeaveBalance>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task InitializeLeaveBalancesForUserAsync_AlreadyInitialized_DoesNotAdd()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _leaveBalanceRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LeaveBalance>
            {
                new LeaveBalance { UserId = userId }
            });

            // Act
            await _leaveBalanceService.InitializeLeaveBalancesForUserAsync(userId);

            // Assert
            _leaveBalanceRepoMock.Verify(r => r.Add(It.IsAny<LeaveBalance>()), Times.Never);
        }

        [Test]
        public async Task DeductLeaveBalanceAsync_SufficientBalance_UpdatesBalance()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leaveTypeId = Guid.NewGuid();
            var balance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LeaveTypeId = leaveTypeId,
                TotalLeaves = 10,
                UsedLeaves = 2
            };

            _leaveBalanceRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LeaveBalance> { balance });

            // Act
            await _leaveBalanceService.DeductLeaveBalanceAsync(userId, leaveTypeId, 3);

            // Assert
            _leaveBalanceRepoMock.Verify(r => r.Update(balance.Id, balance), Times.Once);
            Assert.AreEqual(5, balance.UsedLeaves);
        }

        // [Test]
        // public void DeductLeaveBalanceAsync_InsufficientBalance_ThrowsException()
        // {
        //     // Arrange
        //     var userId = Guid.NewGuid();
        //     var leaveTypeId = Guid.NewGuid();
        //     var balance = new LeaveBalance
        //     {
        //         Id = Guid.NewGuid(),
        //         UserId = userId,
        //         LeaveTypeId = leaveTypeId,
        //         TotalLeaves = 5,
        //         UsedLeaves = 5
        //     };

        //     _leaveBalanceRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LeaveBalance> { balance });

        //     // Act & Assert
        //     var ex = Assert.ThrowsAsync<Exception>(() => _leaveBalanceService.DeductLeaveBalanceAsync(userId, leaveTypeId, 1));
        //     Assert.That(ex.Message, Does.Contain("Insufficient leave balance"));
        // }

        [Test]
        public async Task ResetLeaveBalancesForUserAsync_ResetsUsedLeaves()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var balances = new List<LeaveBalance>
            {
                new LeaveBalance { Id = Guid.NewGuid(), UserId = userId, UsedLeaves = 5 },
                new LeaveBalance { Id = Guid.NewGuid(), UserId = userId, UsedLeaves = 3 }
            };

            _leaveBalanceRepoMock.Setup(r => r.GetAll()).ReturnsAsync(balances);

            // Act
            await _leaveBalanceService.ResetLeaveBalancesForUserAsync(userId);

            // Assert
            _leaveBalanceRepoMock.Verify(r => r.Update(It.IsAny<Guid>(), It.Is<LeaveBalance>(b => b.UsedLeaves == 0)), Times.Exactly(2));
        }
    }
}
