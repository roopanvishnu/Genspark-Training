using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Models;

public class WorkItem
{
    public Guid WorkItemId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public WorkStatus WorkStatus { get; set; } = WorkStatus.Assigned;

    public Guid? AssignedToUserId { get; set; }

    public AppUser? AssignedToUser { get; set; }

    public Guid AssignedByUserId { get; set; }

    public AppUser AssignedByUser { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
