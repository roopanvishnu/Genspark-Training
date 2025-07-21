namespace Bank.Interfaces;

using Bank.Models;
public interface IBankRepository
{
    public Task<BankAccount?> GetAccountByNumberAsync(string accountNumber);
    public Task AddTransactionAsync(Transaction transaction);
    public Task<BankAccount> AddAccountAsync(BankAccount account);
}