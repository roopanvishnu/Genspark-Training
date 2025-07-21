using System;

namespace Notify.Models;

public class Document
{
    public string Id { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public User? UploadedUser { get; set; }
}
