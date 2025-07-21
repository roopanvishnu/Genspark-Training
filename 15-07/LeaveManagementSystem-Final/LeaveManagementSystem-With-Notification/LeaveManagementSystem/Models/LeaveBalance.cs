using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LeaveManagementSystem.Models
{
    public class LeaveBalance
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        public Guid LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }

        [Required]
        public int TotalLeaves { get; set; }

        [Required]
        public int UsedLeaves { get; set; } = 0;

        [NotMapped]
        public int RemainingLeaves => TotalLeaves - UsedLeaves;
    }
}
