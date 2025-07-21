using LeaveManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagementSystem.Models.DTOs;

namespace LeaveManagementSystem.Interfaces
{
    public interface ILeaveBalanceService
    {
        Task<UserLeaveBalanceResponseDto> GetLeaveBalancesForUserAsync(Guid userId);
        Task<UserLeaveBalanceForTypeResponseDto> GetLeaveBalanceForTypeAsync(Guid userId, Guid leaveTypeId);
        Task InitializeLeaveBalancesForUserAsync(Guid userId);
        Task InitializeLeaveBalanceForNewLeaveTypeAsync(Guid userId, Guid leaveTypeId, int standardLeaveCount);
        Task DeductLeaveBalanceAsync(Guid userId, Guid leaveTypeId, int days);
        Task ResetLeaveBalancesForUserAsync(Guid userId);
    }
}
