namespace TwitterCloneAPI.Models;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    
    // Users that this user is following
    public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    
    // Users that follow this user
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
}
