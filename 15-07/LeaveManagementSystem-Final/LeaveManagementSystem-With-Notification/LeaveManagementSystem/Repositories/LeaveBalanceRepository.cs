using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveBalanceRepository : Repository<Guid, LeaveBalance>, IRepository<Guid, LeaveBalance>
    {
        public LeaveBalanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<LeaveBalance> Get(Guid key)
        {
            return await _applicationDbContext.LeaveBalances
                .Include(lb => lb.User)
                .Include(lb => lb.LeaveType)
                .FirstOrDefaultAsync(lb => lb.Id == key);
        }

        public override async Task<IEnumerable<LeaveBalance>> GetAll()
        {
            return await _applicationDbContext.LeaveBalances
                .Include(lb => lb.User)
                .Include(lb => lb.LeaveType)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveBalance>> GetLeaveBalancesForUser(Guid userId)
        {
            return await _applicationDbContext.LeaveBalances
                .Include(lb => lb.LeaveType)
                .Where(lb => lb.UserId == userId)
                .ToListAsync();
        }
    }
}
