namespace BankingApp.Models;

public class Account
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    
    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();
}