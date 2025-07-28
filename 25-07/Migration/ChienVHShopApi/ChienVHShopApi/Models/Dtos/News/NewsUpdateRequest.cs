namespace ChienVHShopAPI.Dtos.News
{
    public class NewsUpdateRequest
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
    }
}
