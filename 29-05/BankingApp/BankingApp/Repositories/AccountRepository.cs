using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Repositories;

public class AccountRepository:IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }
    public async Task<Account> AddAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<IEnumerable<Account>> GetAllAsync() =>await Task.FromResult<IEnumerable<Account>>(_context.Accounts.ToList());

    public async Task<Account?> GetByIdAsync(int id) => await _context.Accounts.FindAsync(id).AsTask();
}