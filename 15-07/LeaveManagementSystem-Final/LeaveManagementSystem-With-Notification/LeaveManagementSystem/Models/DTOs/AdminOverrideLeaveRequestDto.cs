using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class AdminOverrideLeaveRequestDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be 'Approved' or 'Rejected'.")]
        public string NewStatus { get; set; }

        public string? Comment { get; set; }
    }
}
