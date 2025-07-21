using LeaveManagementSystem.Models.DTOs;
using System.Security.Claims;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Interfaces
{

    public interface IAuditLogService
    {
        Task LogAsync(Guid? userId, string action, string entityType, Guid entityId, bool isDeleted = false, DateTime? deletedAt = null);
    }
}
