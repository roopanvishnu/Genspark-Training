using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveRequestCommentRepository : ILeaveRequestCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequestComment> Add(LeaveRequestComment comment)
        {
            _context.LeaveRequestComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<LeaveRequestComment>> GetByLeaveRequestId(Guid leaveRequestId)
        {
            return await _context.LeaveRequestComments
                .Where(c => c.LeaveRequestId == leaveRequestId)
                .Include(c => c.Admin)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
