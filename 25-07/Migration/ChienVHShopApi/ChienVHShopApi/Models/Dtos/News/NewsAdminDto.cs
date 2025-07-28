namespace ChienVHShopAPI.DTOs
{
    public class NewsAdminDto
    {
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }

        // Optional (from User table)
        public string? Username { get; set; }
    }
}
