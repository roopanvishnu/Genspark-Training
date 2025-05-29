namespace BankingApp.Models;

public class Transactions
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    
    public decimal Amount { get; set; }
    public string Type { get; set; } ="Deposit";
    public DateTime OccurredAt { get; set; } = DateTime.Now;
}