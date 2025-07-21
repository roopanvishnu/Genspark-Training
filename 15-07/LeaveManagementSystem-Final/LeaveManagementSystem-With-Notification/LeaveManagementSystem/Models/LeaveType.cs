using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveType
    {
        public Guid Id { get; set; }

        public string Name { get; set; }  

        public string Description { get; set; }

        public int StandardLeaveCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy {get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy {get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<LeaveBalance> LeaveBalances { get; set; }
    }
}
