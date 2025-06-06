using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.Entities;

public class AuditLog:BaseEntity
{
    [Required] [StringLength(100)] public string EntityName { get; set; } = string.Empty;
    [Required] Guid EnitityId { get; set; }
    
    [Required] [StringLength(50)] public string Action { get; set; } = string.Empty; // this refers to create update delete 
    
    public string? OldValues { get; set; }
    
    public string? NewValues { get; set; }
    
    [Required] public Guid UserId { get; set; }
    
    [Required] [StringLength(255)] public string UserEmail { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    [StringLength(45)]
    public string? IpAddress { get; set; }
        
    [StringLength(500)]
    public string? UserAgent { get; set; }
}