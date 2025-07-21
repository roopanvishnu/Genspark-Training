namespace Bank.Models;
public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string BankAccountId { get; set; }//fk

    public BankAccount? BankAccount { get; set; }
}