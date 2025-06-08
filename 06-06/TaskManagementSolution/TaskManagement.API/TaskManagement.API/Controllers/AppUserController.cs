using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Context;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = "Manager")]
public class AppUserController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppUserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .Select(u => new { u.Id, u.Name, u.Email, u.Role })
            .ToListAsync();

        return Ok(new { success = true, message = "Users retrieved", data = users });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(new { success = false, message = "User not found" });

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "User soft deleted" });
    }
}