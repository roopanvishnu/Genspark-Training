using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs 
{
    public class LeaveAttachmentDto
    {
        [Required]
        public Guid LeaveRequestId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }

}