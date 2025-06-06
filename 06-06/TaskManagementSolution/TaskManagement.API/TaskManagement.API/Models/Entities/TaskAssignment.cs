using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.API.Models.Entities;

public class TaskAssignment: BaseEntity
{
    [Required]
    public Guid TaskId { get; set; }
    [ForeignKey("TaskId")] public virtual Task Task { get; set; } = null!;
    
    [Required]
    public Guid AsssignedToUserId { get; set; }
    
    [ForeignKey("AsssignedToUserId")] public virtual User AsssignedToUser { get; set; } = null!;
    
    [Required]
    public Guid AssignedByUserId { get; set; }
    [ForeignKey("AssignedByUserId")] public virtual User AssignedByUser { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UnassignedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    [StringLength(500)]
    public string? Notes { get; set; }
}