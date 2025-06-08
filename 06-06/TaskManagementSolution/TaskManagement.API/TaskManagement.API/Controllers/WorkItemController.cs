using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Context;
using TaskManagement.API.Models;

[ApiController]
[Route("api/v1/workitems")]
public class WorkItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkItemController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAssignedToMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var guid = Guid.Parse(userId);
        var tasks = await _context.WorkItems
            .Where(w => w.IsActive && w.AssignedToUserId == guid)
            .ToListAsync();

        return Ok(new { success = true, data = tasks });
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create(WorkItem model)
    {
        model.WorkItemId = Guid.NewGuid();
        model.IsActive = true;
        model.AssignedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _context.WorkItems.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Work item created", data = model });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Update(Guid id, WorkItem updated)
    {
        var existing = await _context.WorkItems.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = updated.Title;
        existing.Description = updated.Description;
        existing.WorkStatus = updated.WorkStatus;

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var item = await _context.WorkItems.FindAsync(id);
        if (item == null) return NotFound();

        item.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Work item soft deleted" });
    }

    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AssignTask(Guid id, [FromQuery] Guid? userId)
    {
        var item = await _context.WorkItems.FindAsync(id);
        if (item == null) return NotFound();

        item.AssignedToUserId = userId; // if null, considered assigned to all
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = userId != null ? "Assigned to user" : "Assigned to all" });
    }
}
