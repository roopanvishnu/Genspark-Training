namespace TaskManagement.API.DTOs;

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "TeamMember";
}