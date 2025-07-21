using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveAttachmentRepository : Repository<Guid, LeaveAttachment>, IRepository<Guid, LeaveAttachment>
    {
        public LeaveAttachmentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<LeaveAttachment> Get(Guid key)
        {
            return await _applicationDbContext.LeaveAttachments
                .Include(a => a.LeaveRequest)
                .FirstOrDefaultAsync(a => a.Id == key);
        }

        public override async Task<IEnumerable<LeaveAttachment>> GetAll()
        {
            return await _applicationDbContext.LeaveAttachments
                .Include(a => a.LeaveRequest)
                .ToListAsync();
        }
    }

}