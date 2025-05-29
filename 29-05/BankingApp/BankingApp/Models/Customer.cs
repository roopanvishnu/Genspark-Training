namespace BankingApp.Models;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    
    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
   
}