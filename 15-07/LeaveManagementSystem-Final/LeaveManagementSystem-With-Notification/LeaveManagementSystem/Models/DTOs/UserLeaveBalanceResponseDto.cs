using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class UserLeaveBalanceResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<LeaveBalanceResponseDto> LeaveBalances { get; set; }
    }
}
