using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class LeaveRequestDto
    {
        [Required(ErrorMessage = "LeaveTypeID is required.")]
        public Guid LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Start date of leave is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date of leave is required.")]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
