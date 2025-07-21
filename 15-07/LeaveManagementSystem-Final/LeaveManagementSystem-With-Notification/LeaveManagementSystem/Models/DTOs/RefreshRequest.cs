using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs {

    public class RefreshRequest
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; }
    }
}