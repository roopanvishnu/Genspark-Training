using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs;

public class UserRegisterDto
{
    [Required]
    [MaxLength(100)]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(4)]
    public string? Password { get; set; }

    [Required]
    public string? Role { get; set; }
}
