using BankingApp.DTOs;

namespace BankingApp.Interfaces;

public interface IAccountService
{
    Task<AccountDto> CreateAccountAsync(AccountDto accountDto );
    Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
}