namespace Bank.Services;

using Bank.Interfaces;
using Bank.Models;
using Bank.Repositories;

public class BankService : IBankService
{
    private readonly IBankRepository _bankRepository;

    public BankService(IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<decimal> GetBalanceAsync(string accountNumber)
    {
        var account = await _bankRepository.GetAccountByNumberAsync(accountNumber);
        if(account == null)
            throw new InvalidOperationException("Account not found.");
        return account.Balance;
    }

    public async Task<bool> DepositAsync(string accountId, decimal amount)
    {
        var account = await _bankRepository.GetAccountByNumberAsync(accountId);
        if (account == null || amount <= 0) return false;

        account.Balance += amount;
        await _bankRepository.AddTransactionAsync(new Transaction
        {
            BankAccountId = accountId,
            Amount = amount,
            Type = "deposit"
        });
        return true;
    }

    public async Task<bool> WithdrawAsync(string accountId, decimal amount)
    {
        var account = await _bankRepository.GetAccountByNumberAsync(accountId);
        if (account == null || amount <= 0 || account.Balance < amount) return false;

        account.Balance -= amount;
        await _bankRepository.AddTransactionAsync(new Transaction
        {
            BankAccountId = accountId,
            Amount = -amount,
            Type = "withdraw"
        });
        return true;
    }

    public async Task<BankAccount> CreateAccountAsync(BankAccount account)
    {
        var existing = await _bankRepository.GetAccountByNumberAsync(account.AccountNumber);
        if (existing != null)
            throw new InvalidOperationException("Account number already exists.");
        return await _bankRepository.AddAccountAsync(account);
    }
}