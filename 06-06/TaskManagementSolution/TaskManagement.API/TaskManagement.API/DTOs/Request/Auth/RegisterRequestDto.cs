using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.DTOs.Request.Auth;

public class RegisterRequestDto
{
    // first name validation
    [Required(ErrorMessage = "First Name is Required")]
    [StringLength(100, ErrorMessage = "First Name cannot be exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;
    
    // last name validation
    [Required(ErrorMessage = "Last Name is Required")]
    [StringLength(100, ErrorMessage = "Last Name cannot be exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;
    
    // email validation
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [StringLength(255, ErrorMessage = "Email cannot be exceed 255 characters")]
    public string Email { get; set; } = string.Empty;
    
    // Password validation
    [Required(ErrorMessage = "Password is Required")]
    [MinLength(4, ErrorMessage = "password must have at least 4 characters")]
    [StringLength(100, ErrorMessage = "Password cannot be exceed 100 characters")]
    public string Password { get; set; } = string.Empty;
    
    // Role validation
    [Required(ErrorMessage = "Role is required")]
    public UserRole Role { get; set; }
    
    // phone validation
    [Required(ErrorMessage = "Please enter valid phone number")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed more than 12 characters including country code")]
    public string? PhoneNumber { get; set; }
}