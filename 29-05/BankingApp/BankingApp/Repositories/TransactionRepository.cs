using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext _context;

    public TransactionRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<Transactions> AddAsync(Transactions transaction)
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return transaction;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
    public Task<IEnumerable<Transactions>> GetByAccountIdAsync(int accountId) =>
        Task.FromResult<IEnumerable<Transactions>>(
            _context.Transactions.Where(t => t.AccountId == accountId).ToList()
        );

    public async Task<object> GetByAccountIdFromSPAsync(int accountId)
    {
        return await _context.Transactions
            .FromSqlRaw("SELECT * FROM GetTransactionsByAccountId({0})", accountId)
            .ToListAsync();
    }
}