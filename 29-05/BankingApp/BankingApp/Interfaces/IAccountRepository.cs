using BankingApp.Models;

namespace BankingApp.Interfaces;

public interface IAccountRepository
{
    Task<Account> AddAsync (Account account);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
}