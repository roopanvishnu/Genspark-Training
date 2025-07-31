namespace ChienVHShopOnline.DTOs;

public class NewsAddDTO
{
    public int? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public System.DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? Status { get; set; }
}