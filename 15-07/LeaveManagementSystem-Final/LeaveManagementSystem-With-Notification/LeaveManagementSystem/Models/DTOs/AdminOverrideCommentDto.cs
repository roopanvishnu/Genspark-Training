using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class AdminOverrideCommentDto
    {
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
