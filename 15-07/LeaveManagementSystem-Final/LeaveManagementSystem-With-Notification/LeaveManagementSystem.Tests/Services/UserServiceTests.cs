using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Services;
using LeaveManagementSystem.Helpers;
using System.Linq;

namespace LeaveManagementSystem.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<Guid, User>> _userRepositoryMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<ILeaveBalanceService> _leaveBalanceServiceMock; 
        private Mock<IMapper> _mapperMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IRepository<Guid, User>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _leaveBalanceServiceMock = new Mock<ILeaveBalanceService>(); 
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _auditLogServiceMock.Object,
                _currentUserServiceMock.Object,
                _leaveBalanceServiceMock.Object, 
                _mapperMock.Object);
        }

        [Test]
        public async Task GetUserById_UserExists_ReturnsUserDto()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var userDto = new UserDto { Id = userId };

            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            var result = await _userService.GetUserById(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
        }

        [Test]
        public void GetUserById_UserNotFound_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userService.GetUserById(userId);
            });
        }

        [Test]
        public async Task CreateUser_ValidUser_ReturnsCreatedUserDto()
        {
            var createUserDto = new CreateUserDto { Username = "Test", Password = "password" };
            var user = new User { Id = Guid.NewGuid(), Username = "Test" };
            var createdUserDto = new UserDto { Id = user.Id, Username = "Test" };

            _mapperMock.Setup(m => m.Map<User>(createUserDto)).Returns(user);
            _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(createdUserDto);

            var result = await _userService.CreateUser(createUserDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Username);
        }

        [Test]
        public async Task UpdateUser_UserExists_ReturnsUpdatedUserDto()
        {
            var userId = Guid.NewGuid();
            var updateUserDto = new UpdateUserDto { Username = "Updated" };
            var existingUser = new User { Id = userId, Username = "Old" };
            var updatedUser = new User { Id = userId, Username = "Updated" };
            var updatedUserDto = new UserDto { Id = userId, Username = "Updated" };

            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.Update(userId, existingUser)).ReturnsAsync(updatedUser);
            _mapperMock.Setup(m => m.Map(updateUserDto, existingUser)).Returns(updatedUser);
            _mapperMock.Setup(m => m.Map<UserDto>(updatedUser)).Returns(updatedUserDto);

            var result = await _userService.UpdateUser(userId, updateUserDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", result.Username);
        }

        [Test]
        public void UpdateUser_UserNotFound_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            var updateUserDto = new UpdateUserDto();

            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userService.UpdateUser(userId, updateUserDto);
            });
        }

        [Test]
        public async Task DeleteUser_UserExists_ReturnsDeletedUserDto()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId };
            var deletedUser = new User { Id = userId };
            var deletedUserDto = new UserDto { Id = userId };

            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.Delete(userId)).ReturnsAsync(deletedUser);
            _mapperMock.Setup(m => m.Map<UserDto>(deletedUser)).Returns(deletedUserDto);

            var result = await _userService.DeleteUser(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
        }

        [Test]
        public void DeleteUser_UserNotFound_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userService.DeleteUser(userId);
            });
        }
    }
}
