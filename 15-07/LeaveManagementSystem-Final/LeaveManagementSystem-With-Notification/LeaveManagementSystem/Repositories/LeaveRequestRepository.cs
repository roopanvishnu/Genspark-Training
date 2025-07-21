using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveRequestRepository : Repository<Guid, LeaveRequest>, IRepository<Guid, LeaveRequest>
    {
        public LeaveRequestRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<LeaveRequest> Get(Guid key)
        {
            return await _applicationDbContext.LeaveRequests
                .Include(r => r.User)
                .Include(r => r.LeaveType)
                .Include(r => r.ReviewedBy)
                .Include(r => r.Attachments)
                .FirstOrDefaultAsync(r => r.Id == key);
        }

        public override async Task<IEnumerable<LeaveRequest>> GetAll()
        {
            return await _applicationDbContext.LeaveRequests
                .Include(r => r.User)
                .Include(r => r.LeaveType)
                .Include(r => r.ReviewedBy)
                .Include(r => r.Attachments)
                .ToListAsync();
        }
    }

}