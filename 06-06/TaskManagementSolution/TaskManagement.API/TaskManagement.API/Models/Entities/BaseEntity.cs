using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get;set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } =string.Empty;
    public DateTime? UpdateAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}