namespace TaskManagement.API.Repositories.Interfaces.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    IUserRepository Users { get; }
    ITaskRepository Tasks { get; }
    ITaskAssignmentRepository TaskAssignments { get; }
    IFileAttachmentRepository FileAttachments { get; }
    IAuditLogRepository AuditLogs { get; }
        
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}