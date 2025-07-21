namespace Bank.Models;

public class BankAccount
{
    public int Id { get; set; }
    public string AccountHolderName { get; set; }
    public string AccountNumber { get; set; } 
    public decimal Balance { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Transaction>? Transactions { get; set; }
}