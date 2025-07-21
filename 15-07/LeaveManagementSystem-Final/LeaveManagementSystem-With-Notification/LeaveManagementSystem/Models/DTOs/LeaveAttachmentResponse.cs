using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models.DTOs {
    public class LeaveAttachmentResponse
    {
        public Guid Id { get; set; }
        public Guid LeaveRequestId { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public string DownloadUrl { get; set; }
    }

}