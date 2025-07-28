namespace ChienVHShopAPI.Dtos
{
    public class ContactUsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string RecaptchaResponse { get; set; } = string.Empty;
    }
}