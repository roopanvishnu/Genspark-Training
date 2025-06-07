using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;
namespace TaskManagement.API.Models.Entities;

public class User:BaseEntity
{
    [Required] [StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required] [StringLength(100)] public string LastName { get; set; } = string.Empty;
    [Required] [EmailAddress][StringLength(255)] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    [Required] public UserRole Role { get; set; } 
    [StringLength(15)] public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    //Navigation
    public virtual ICollection<Task> CreateTasks { get; set; } = new List<Task>();
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}