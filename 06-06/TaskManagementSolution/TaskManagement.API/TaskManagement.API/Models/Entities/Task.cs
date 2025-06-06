using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.API.Models.Entities;

public class Task : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = String.Empty;
    [StringLength(1000)]
    public string Description { get; set; } = String.Empty;

    [Required] public TaskManagement.API.Models.Enums.TaskStatus Status { get; set; } = TaskManagement.API.Models.Enums.TaskStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskManagement.API.Models.Enums.TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    [Required]
    public Guid CreatedById { get; set; }

    [ForeignKey(("CreatedById"))] public virtual User CreatedBy { get; set; } = null!;
    
    //navigation
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
    public virtual ICollection<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();
}