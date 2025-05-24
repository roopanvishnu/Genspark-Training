namespace SecureFileAccessApp.Models;

public class User
{
    public string UserName { get; set; }  
    public string Role {  get; set; }

    public User(string username, string role)
    {
        UserName = username;
        Role = role;    
    }
}