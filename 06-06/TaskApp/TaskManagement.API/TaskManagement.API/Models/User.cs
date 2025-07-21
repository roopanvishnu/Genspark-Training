namespace TaskManagement.API.Models;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; } // Unique
    public string PasswordHash { get; set; }
    public string Role { get; set; } // Manager, TeamMember

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public ICollection<TaskItem> AssignedTasks { get; set; }
    
    
    //relation for refresh token
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

}
