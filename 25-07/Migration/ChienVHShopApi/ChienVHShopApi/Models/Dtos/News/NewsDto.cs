namespace ChienVHShopAPI.DTOs
{
    public class NewsDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Image { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}