namespace ChienVHShopOnline.DTOs;
public class LoginResponseDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}