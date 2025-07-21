using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Models.DTOs
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<LeaveBalanceResponseDto> LeaveBalances { get; set; }
        public List<LeaveRequestResponseDto> LeaveRequests { get; set; }

    }
}
