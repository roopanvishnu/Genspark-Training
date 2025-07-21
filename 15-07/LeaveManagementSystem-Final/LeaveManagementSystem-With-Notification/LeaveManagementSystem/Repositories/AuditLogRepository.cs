using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class AuditLogRepository : Repository<Guid, AuditLog>, IRepository<Guid, AuditLog>
    {
        public AuditLogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<AuditLog> Get(Guid key)
        {
            return await _applicationDbContext.AuditLogs.FirstOrDefaultAsync(a => a.Id == key);
        }

        public override async Task<IEnumerable<AuditLog>> GetAll()
        {
            return await _applicationDbContext.AuditLogs.ToListAsync();
        }
    }

}