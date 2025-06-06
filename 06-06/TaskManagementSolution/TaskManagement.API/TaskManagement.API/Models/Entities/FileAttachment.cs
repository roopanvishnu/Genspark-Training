using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.API.Models.Entities;

public class FileAttachment:BaseEntity
{
    [Required] [StringLength(255)] public string FileName { get; set; } = string.Empty;
    [Required] [StringLength(255)] public string OriginalFileName { get; set; } = string.Empty;
    [Required] [StringLength(255)] public string ContentType { get; set; } = string.Empty;
    [Required] public long FileSizeBytes { get; set; }
    [Required] [StringLength(500)] public string FilePath { get; set; } = string.Empty;
    [Required] public Guid TaskId { get; set; }
    [ForeignKey("TaskId")] public virtual Task Task { get; set; } = null!;
    [Required] public Guid UploadedByUserId { get; set; }
    [ForeignKey("UploadedByUserId")] public virtual User UploadedByUser { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}