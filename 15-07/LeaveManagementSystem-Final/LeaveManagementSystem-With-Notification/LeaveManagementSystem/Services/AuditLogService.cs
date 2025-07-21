using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;

namespace LeaveManagementSystem.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<Guid, AuditLog> _auditLogRespository;
        public AuditLogService(IRepository<Guid, AuditLog> auditLogRespository) {
            _auditLogRespository = auditLogRespository;
        }

        public Task LogAsync(Guid? userId, string action, string entityType, Guid entityId, bool isDeleted = false, DateTime? deletedAt = null)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    IsDeleted = isDeleted,
                    DeletedAt = deletedAt,
                    Timestamp = DateTime.UtcNow
                };
                _auditLogRespository.Add(log);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in audit logging", ex);
            }
            return Task.CompletedTask;
        }
    }
}