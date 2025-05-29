namespace BankingApp.Models;

public class CustomerAccount
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public int AccountId { get; set; }
    public Account? Account { get; set; }
}