using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class UserRepository : Repository<Guid, User>, IRepository<Guid, User>
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<User> Get(Guid key)
        {
            return await _applicationDbContext.Users
                .Include(u => u.LeaveBalances)
                    .ThenInclude(lb => lb.LeaveType)
                .Include(u => u.LeaveRequests)
                    .ThenInclude(lr => lr.LeaveType)
                .Include(u => u.LeaveRequests)
                    .ThenInclude(lr => lr.ReviewedBy)
                .FirstOrDefaultAsync(u => u.Id == key && !u.IsDeleted);
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _applicationDbContext.Users.ToListAsync();
        }
    }
}
