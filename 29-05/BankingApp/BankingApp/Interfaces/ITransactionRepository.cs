using BankingApp.Models;
namespace BankingApp.Interfaces;

public interface ITransactionRepository
{
    Task<Transactions> AddAsync(Transactions transaction);
    Task<IEnumerable<Models.Transactions>> GetByAccountIdAsync(int accountId);
    Task<object> GetByAccountIdFromSPAsync(int accountId);
}