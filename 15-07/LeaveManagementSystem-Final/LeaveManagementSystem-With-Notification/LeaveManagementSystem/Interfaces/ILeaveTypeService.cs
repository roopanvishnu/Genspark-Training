using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<IEnumerable<LeaveTypeResponseDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm, string sortBy, string sortOrder);
        Task<LeaveType> GetByIdAsync(Guid id);
        Task<LeaveType> CreateAsync(LeaveTypeDto dto);
        Task<LeaveType> UpdateAsync(Guid id, LeaveTypeDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
