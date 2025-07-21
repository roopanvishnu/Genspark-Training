using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Employee|HR)$", ErrorMessage = "Role must be either 'Employee' or 'HR'.")]
        public string Role { get; set; } 

        [RegularExpression("^(?i)(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
        public string Gender { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
