using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public UserController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET /api/v1/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _authService.GetCurrentUserAsync(HttpContext);
            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                success = true,
                message = "Authenticated user",
                data = user
            });
        }

        // PUT /api/v1/users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId != id.ToString() && currentUserRole != "Manager")
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound(new { success = false, message = "User not found" });

            user.FullName = dto.FullName;
            if (currentUserRole == "Manager") // Only Manager can change role
                user.Role = dto.Role;

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUserId;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "User updated successfully" });
        }

        // DELETE /api/v1/users/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound(new { success = false, message = "User not found" });

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "User soft-deleted successfully" });
        }
    }
}
