namespace BankingApp.Models;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}