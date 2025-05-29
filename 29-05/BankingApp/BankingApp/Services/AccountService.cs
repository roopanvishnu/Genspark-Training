using BankingApp.DTOs;
using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo)
    {
        _repo = repo;
    }

    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto)
    {
        var account = new Account { Balance = dto.InitialBalance, BranchId = dto.BranchId };
        account = await _repo.AddAsync(account);
        return new AccountDto { Id = account.Id, Balance = account.Balance, BranchId = account.BranchId };
    }

    public async Task<AccountDto> CreateAccountAsync(AccountDto accountDto)
    {
        var account = new Account
        {
            Balance = accountDto.Balance,
            BranchId = accountDto.BranchId
        };

        account = await _repo.AddAsync(account);

        return new AccountDto
        {
            Id = account.Id,
            Balance = account.Balance,
            BranchId = account.BranchId
        };
    }


    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        var accounts = await _repo.GetAllAsync();
        return accounts.Select(a => new AccountDto { Id = a.Id, Balance = a.Balance, BranchId = a.BranchId });
    }
}