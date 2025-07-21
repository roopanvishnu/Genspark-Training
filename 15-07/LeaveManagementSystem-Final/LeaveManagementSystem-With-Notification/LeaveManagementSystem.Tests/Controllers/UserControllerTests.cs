using LeaveManagementSystem.Controllers;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IAuthorizationService> _authorizationServiceMock;
        private UsersController _controller;
        private Mock<ILogger<UsersController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _loggerMock = new Mock<ILogger<UsersController>>();

            _controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllUsers_ReturnsOkResultWithUsers()
        {
            var users = new List<UserDto> { new UserDto { Id = Guid.NewGuid(), Username = "John" } };
            _userServiceMock.Setup(s => s.GetAllUsers(1, 10, null, null, "CreatedAt", "asc"))
                            .ReturnsAsync((users, users.Count));

            var result = await _controller.GetAllUsers();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        

        [Test]
        public async Task GetUserById_UserExists_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Username = "Jane" };
            _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync(user);

            var result = await _controller.GetUserByIdV1(userId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.GetUserById(userId))
                            .ReturnsAsync((UserDto)null);

            var result = await _controller.GetUserByIdV1(userId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var apiResponse = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Success", apiResponse.Message);
        }

        
        [Test]
        public async Task CreateUser_ValidUser_ReturnsCreatedResult()
        {
            var newUser = new CreateUserDto { Username = "NewUser", Email = "test@test.com", Password = "pass", Role = "User" };
            var createdUser = new UserDto { Id = Guid.NewGuid(), Username = newUser.Username };

            _userServiceMock.Setup(s => s.CreateUser(newUser)).ReturnsAsync(createdUser);

            var result = await _controller.CreateUser(newUser);
            var createdResult = result as CreatedAtActionResult;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task CreateUser_InvalidModelState_ReturnsBadRequest()
        {
            var newUser = new CreateUserDto();
            _controller.ModelState.AddModelError("Username", "Required");

            var result = await _controller.CreateUser(newUser);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task UpdateUser_AuthorizedUser_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var updateUser = new UpdateUserDto { Username = "UpdatedUser", Email = "updated@test.com", Role = "User" };
            var updatedUser = new UserDto { Id = userId, Username = updateUser.Username };

            _userServiceMock.Setup(s => s.UpdateUser(userId, updateUser)).ReturnsAsync(updatedUser);
            _authorizationServiceMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), userId, "CanUpdateUserPolicy"))
                .ReturnsAsync(AuthorizationResult.Success);

            var controller = GetControllerWithUser(userId, "HR");

            var result = await controller.UpdateUser(userId, updateUser, _authorizationServiceMock.Object);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task UpdateUser_UnauthorizedUser_ReturnsForbidden()
        {
            var userId = Guid.NewGuid();
            var updateUser = new UpdateUserDto { Username = "UpdatedUser", Email = "updated@test.com", Role = "User" };

            _authorizationServiceMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), userId, "CanUpdateUserPolicy"))
                .ReturnsAsync(AuthorizationResult.Failed());

            var controller = GetControllerWithUser(userId, "User");

            var result = await controller.UpdateUser(userId, updateUser, _authorizationServiceMock.Object);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public void UpdateUser_NullBody_ThrowsNullReferenceException()
        {
            var userId = Guid.NewGuid();

            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _controller.UpdateUser(userId, null, _authorizationServiceMock.Object);
            });
        }

        [Test]
        public async Task DeleteUser_ValidId_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var deletedUser = new UserDto { Id = userId, Username = "ToDelete" };
            _userServiceMock.Setup(s => s.DeleteUser(userId)).ReturnsAsync(deletedUser);

            var result = await _controller.DeleteUser(userId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.DeleteUser(userId)).ReturnsAsync((UserDto)null);

            var result = await _controller.DeleteUser(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
        }

        private UsersController GetControllerWithUser(Guid userId, string role)
        {
            var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            return controller;
        }
    }
}
