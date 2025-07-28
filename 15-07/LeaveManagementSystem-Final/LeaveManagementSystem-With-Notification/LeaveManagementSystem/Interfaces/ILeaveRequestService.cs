using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<(IEnumerable<LeaveRequestResponseDto> Requests, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string searchTerm, string status, string sortBy, string sortOrder);
        Task<LeaveRequestResponseDto> GetByIdAsync(Guid id);
        Task<LeaveRequestResponseDto> CreateAsync(LeaveRequestDto request);
        Task<bool> UpdateStatusAsync(Guid id, string status, Guid approverId);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> CancelLeaveRequestAsync(Guid id);
        Task<bool> AdminOverrideLeaveStatusAsync(Guid leaveRequestId, string newStatus, Guid adminId, string? comment = null);
        // In ILeaveRequestService.cs
Task<AdminOverrideCommentDto?> GetAdminOverrideCommentAsync(Guid leaveRequestId);

    }
}
