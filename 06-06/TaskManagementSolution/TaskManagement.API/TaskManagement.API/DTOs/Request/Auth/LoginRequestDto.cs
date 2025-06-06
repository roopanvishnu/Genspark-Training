using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Request.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Emaiil is required")]
    [EmailAddress(ErrorMessage = "Invalid email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(4, ErrorMessage = "password must have at least 4 characters")]
    public string password { get; set; } = string.Empty;
}