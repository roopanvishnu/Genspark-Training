using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Models;

public class AppUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public bool IsActive { get; set; } = true;

    public ICollection<WorkItem> AssignedTasks { get; set; } = new List<WorkItem>();      // Tasks assigned by this user
    public ICollection<WorkItem> TasksAssignedToMe { get; set; } = new List<WorkItem>();  // Tasks received by this user
}
