namespace Bank.Interfaces;

using Bank.Models;
public interface IBankService
{
    public Task<decimal> GetBalanceAsync(string accountNumber);
    public Task<bool> DepositAsync(string accountId, decimal amount);
    public Task<bool> WithdrawAsync(string accountId, decimal amount);
    public Task<BankAccount> CreateAccountAsync(BankAccount account);
}