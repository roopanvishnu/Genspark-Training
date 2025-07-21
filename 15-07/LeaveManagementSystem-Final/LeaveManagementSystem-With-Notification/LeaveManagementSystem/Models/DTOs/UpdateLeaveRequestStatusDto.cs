using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class UpdateLeaveRequestStatusDto
    {
        [Required]
        [RegularExpression("Approved|Rejected", ErrorMessage = "Status must be either 'Approved' or 'Rejected'.")]
        public string Status { get; set; }
    }
}
