using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class LeaveTypeDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public int StandardLeaveCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
