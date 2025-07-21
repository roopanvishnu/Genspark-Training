using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid RecipientId { get; set; }  
        public User Recipient { get; set; }

        public string Message { get; set; } 

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public Guid? ReviewedById { get; set; }
        public User? ReviewedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
