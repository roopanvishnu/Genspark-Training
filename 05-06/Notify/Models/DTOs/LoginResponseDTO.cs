using System;

namespace Notify.Models.DTOs;

public class LoginResponseDTO
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
