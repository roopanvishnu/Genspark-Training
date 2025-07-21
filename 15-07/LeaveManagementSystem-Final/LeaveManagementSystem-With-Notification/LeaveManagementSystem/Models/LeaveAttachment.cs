using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveAttachment
    {
        public Guid Id { get; set; }

        public Guid LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; }

        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public DateTime UploadedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
