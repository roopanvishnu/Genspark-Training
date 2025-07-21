using System;
using System.Threading.Tasks;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Interfaces
{
    public interface ILeaveRequestCommentRepository
    {
        Task<LeaveRequestComment> Add(LeaveRequestComment comment);
        Task<IEnumerable<LeaveRequestComment>> GetByLeaveRequestId(Guid leaveRequestId);
    }
}
