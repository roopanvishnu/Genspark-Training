namespace Bank.Repositories;

using Bank.Models;
using Bank.Interfaces;
using Bank.Contexts;
using Bank.Services;
using Microsoft.EntityFrameworkCore;
public class BankRepositories : IBankRepository
{
    private readonly BankContext _bankContext;

    public BankRepositories(BankContext bankContext)
    {
        _bankContext = bankContext;
    }

    public async Task<BankAccount?> GetAccountByNumberAsync(string accountNumber)
    {
        return await _bankContext.BankAccounts
            .SingleOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _bankContext.Transactions.AddAsync(transaction);
        await _bankContext.SaveChangesAsync();
    }

    public async Task<BankAccount> AddAccountAsync(BankAccount account)
    {
        await _bankContext.BankAccounts.AddAsync(account);
        await _bankContext.SaveChangesAsync();
        return account;
    }
}