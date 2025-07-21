using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveTypeRepository : Repository<Guid, LeaveType>, IRepository<Guid, LeaveType>
    {
        public LeaveTypeRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<LeaveType> Get(Guid key)
        {
            return await _applicationDbContext.LeaveTypes.FirstOrDefaultAsync(l => l.Id == key);
        }

        public override async Task<IEnumerable<LeaveType>> GetAll()
        {
            return await _applicationDbContext.LeaveTypes.ToListAsync();
        }
    }

}