using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs;
using TaskManagement.API.Filters;
using TaskManagement.API.Models;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [ServiceFilter(typeof(CustomExceptionFilter))]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, IAuthService authService, ILogger<UserController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }
        [HttpGet("test-exception")]
        public IActionResult TestException()
        {
            throw new Exception("This is a test exception from controller.");
        }


        // GET /api/v1/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            _logger.LogInformation("Fetching current user info...");
            var user = await _authService.GetCurrentUserAsync(HttpContext);
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return Unauthorized();
            }

            _logger.LogInformation("Successfully fetched current user info for user ID: {UserId}", user.Id);
            return Ok(new
            {
                success = true,
                message = "Authenticated user",
                data = user
            });
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found or deleted for ID: {UserId}", id);
                return NotFound(new { success = false, message = "User not found" });
            }

            var result = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            _logger.LogInformation("Successfully fetched user details for ID: {UserId}", id);
            return Ok(new { success = true, message = "User details fetched", data = result });
        }

        // pagination
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching users list with page: {Page}, pageSize: {PageSize}", page, pageSize);
            
            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.FullName);

            var totalRecords = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            });

            _logger.LogInformation("Successfully fetched {UserCount} users out of {TotalRecords} total records", users.Count, totalRecords);
            return Ok(new
            {
                success = true,
                message = "User list fetched",
                data = result,
                pagination = new
                {
                    totalRecords,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
                }
            });
        }

        // PUT /api/v1/users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            _logger.LogInformation("Attempting to update user ID: {UserId}", id);
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId != id.ToString() && currentUserRole != "Manager")
            {
                _logger.LogWarning("Unauthorized attempt to update user ID: {UserId} by user: {CurrentUserId}", id, currentUserId);
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found or deleted for update operation, ID: {UserId}", id);
                return NotFound(new { success = false, message = "User not found" });
            }

            user.FullName = dto.FullName;
            if (currentUserRole == "Manager") // Only Manager can change role
                user.Role = dto.Role;

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUserId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated user ID: {UserId} by user: {CurrentUserId}", id, currentUserId);
            return Ok(new { success = true, message = "User updated successfully" });
        }

        // DELETE /api/v1/users/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(Guid id)
        {
            _logger.LogInformation("Attempting to soft delete user ID: {UserId}", id);
            
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found or already deleted for soft delete operation, ID: {UserId}", id);
                return NotFound(new { success = false, message = "User not found" });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUserId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully soft deleted user ID: {UserId} by manager: {ManagerId}", id, currentUserId);
            return Ok(new { success = true, message = "User soft-deleted successfully" });
        }
    }
}