using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;  
using LeaveManagementSystem.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger; 

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string role = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "asc")
        {
            pageSize = pageSize > 100 ? 100 : pageSize;

            try
            {
                _logger.LogInformation("Fetching all users with pagination parameters.");
                var (users, totalCount) = await _userService.GetAllUsers(page, pageSize, searchTerm, role, sortBy, sortOrder);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var response = new
                {
                    data = users,
                    pagination = new
                    {
                        totalRecords = totalCount,
                        page,
                        pageSize,
                        totalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users.");
                var response = ApiResponse<object>.FailureResponse("Failed to get users", new Dictionary<string, List<string>>
                {
                    { "Exception", new List<string> { ex.Message } }
                });
                return StatusCode(500, response);
            }
        }

        // Version 1 - Original GetUserById
        [HttpGet("user/{id:guid}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetUserByIdV1(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", id);
                var user = await _userService.GetUserById(id);
                var response = ApiResponse<object>.SuccessResponse(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", id);
                var response = ApiResponse<object>.FailureResponse("Failed to get user by id", new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } });
                return NotFound(response);
            }
        }

        // Version 2 - Simplified response
        [HttpGet("user/{id:guid}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetUserByIdV2(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching V2 user with ID: {UserId}", id);
                var user = await _userService.GetUserById(id);

                // Return only selected fields
                var response = ApiResponse<object>.SuccessResponse(new
                {
                    user.Id,
                    user.Username,
                    user.Email
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in V2 while fetching user with ID: {UserId}", id);
                var response = ApiResponse<object>.FailureResponse("Failed to get user by id (V2)", new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } });
                return NotFound(response);
            }
        }

        

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail(Guid id, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var result = await _userService.GetUserDetailById(id, skip, take);
            return Ok(result);
        }


        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user creation request received.");
                var response = ApiResponse<object>.FailureResponse("Invalid request", ModelStateErrors());
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("Creating new user.");
                var createdUser = await _userService.CreateUser(createUserDto);
                var successResponse = ApiResponse<object>.SuccessResponse(createdUser, "User created successfully");
                return CreatedAtAction(nameof(GetUserByIdV1), new { id = createdUser.Id }, successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a user.");
                var response = ApiResponse<object>.FailureResponse($"Failed to create user", new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } });
                return StatusCode(500, response);
            }
        }


        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto, [FromServices] IAuthorizationService authorizationService)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user update request received.");
                var response = ApiResponse<object>.FailureResponse("Invalid request", ModelStateErrors());
                return BadRequest(response);
            }

            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "CanUpdateUserPolicy");

            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning("User is not authorized to update user with ID: {UserId}", id);
                return Forbid();
            }

            try
            {
                _logger.LogInformation("Updating user with ID: {UserId}", id);
                var updatedUser = await _userService.UpdateUser(id, updateUserDto);
                var response = ApiResponse<object>.SuccessResponse(updatedUser, "User updated successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", id);
                var response = ApiResponse<object>.FailureResponse(ex.Message);
                return NotFound(response);
            }
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {UserId}", id);
                var deletedUser = await _userService.DeleteUser(id);
                var response = ApiResponse<object>.SuccessResponse(deletedUser, "User deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                var response = ApiResponse<object>.FailureResponse(ex.Message);
                return NotFound(response);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(dto);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Password updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password.");
                return StatusCode(500, ApiResponse<object>.FailureResponse("Internal server error"));
            }
        }


        private Dictionary<string, List<string>> ModelStateErrors()
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var key in ModelState.Keys)
            {
                var errorMessages = ModelState[key].Errors;
                if (errorMessages.Count > 0)
                {
                    errors[key] = new List<string>();
                    foreach (var error in errorMessages)
                    {
                        errors[key].Add(error.ErrorMessage);
                    }
                }
            }
            return errors;
        }
    }
}
