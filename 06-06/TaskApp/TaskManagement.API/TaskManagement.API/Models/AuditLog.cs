namespace TaskManagement.API.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }

    public string ActionType { get; set; } = string.Empty;// Created, Updated, Deleted
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }

    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
}
