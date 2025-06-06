using TaskManagement.API.Models.Entities;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IAuditLogRepository:IGenericRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, Guid entityId);
    Task<IEnumerable<AuditLog>> GetLogsByUserAsync(Guid userId);
    Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime fromDate, DateTime toDate);
}