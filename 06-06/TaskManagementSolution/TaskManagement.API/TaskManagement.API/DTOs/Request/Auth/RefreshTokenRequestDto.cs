using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Request.Auth;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Access Token is Required")]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Refresh Token is Required")]
    public string RefreshToken { get; set; } = string.Empty;
}