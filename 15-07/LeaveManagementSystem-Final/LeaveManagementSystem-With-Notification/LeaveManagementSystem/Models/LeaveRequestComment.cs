using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models
{
    public class LeaveRequestComment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeaveRequestId { get; set; }

        [ForeignKey(nameof(LeaveRequestId))]
        public LeaveRequest LeaveRequest { get; set; }

        [Required]
        public Guid AdminId { get; set; }

        [ForeignKey(nameof(AdminId))]
        public User Admin { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
