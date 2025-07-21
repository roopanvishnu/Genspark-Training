using System;

namespace Notify.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[]? Password { get; set; }
    public byte[]? HashKey { get; set; }

    public List<Document>? Documents { get; set; }
}
