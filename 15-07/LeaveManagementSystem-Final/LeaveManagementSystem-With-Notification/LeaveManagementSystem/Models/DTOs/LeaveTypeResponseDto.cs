using System;

namespace LeaveManagementSystem.Models.DTOs
{
    public class LeaveTypeResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int StandardLeaveCount { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
