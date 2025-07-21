using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{

    public class UserLeaveBalanceForTypeResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public LeaveBalanceResponseDto LeaveBalance { get; set; }
    }
}
