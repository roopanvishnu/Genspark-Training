using LeaveManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagementSystem.Models.DTOs;

namespace LeaveManagementSystem.Interfaces
{
    public interface ILeaveAttachmentService
    {
        Task<IEnumerable<LeaveAttachment>> GetByLeaveRequestIdAsync(Guid leaveRequestId);
        Task<LeaveAttachment> GetByIdAsync(Guid id);
        Task<LeaveAttachment> CreateAsync(LeaveAttachmentDto attachment);
        Task<bool> DeleteAsync(Guid id);
    }
}
