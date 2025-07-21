using System;

namespace LeaveManagementSystem.Models.DTOs
{
    public class LeaveBalanceResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }  
        public int TotalLeaves { get; set; }
        public int UsedLeaves { get; set; }
        public int RemainingLeaves { get; set; }
    }
}
