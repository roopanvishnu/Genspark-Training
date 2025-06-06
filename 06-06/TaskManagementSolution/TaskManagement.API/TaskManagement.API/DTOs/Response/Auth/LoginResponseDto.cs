namespace TaskManagement.API.DTOs.Response.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserProfileResponseDto User { get; set; } = new UserProfileResponseDto();
}
