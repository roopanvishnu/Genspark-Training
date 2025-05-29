using BankingApp.DTOs;
using BankingApp.Interfaces;
using BankingApp.Models;
using System.Linq;

namespace BankingApp.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;

    public TransactionService(ITransactionRepository repo)
    {
        _repo = repo;
    }

    public async Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = new Transactions
        {
            AccountId = transactionDto.AccountId,
            Amount = transactionDto.Amount,
            Type = transactionDto.Type,
            OccurredAt = transactionDto.OccurredAt
        };
        
        transaction = await _repo.AddAsync(transaction);

        return new TransactionDto
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            OccurredAt = transaction.OccurredAt
        };
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        // Optional: implement if you have a GetAllAsync method in your repository
        throw new NotImplementedException();
    }

    public async Task<object?> GetTransactionsForAccountAsync(int accountId)
    {
        var transactions = await _repo.GetByAccountIdAsync(accountId);
        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Amount = t.Amount,
            Type = t.Type,
            OccurredAt = t.OccurredAt
        });
    }

    public async Task<object?> GetTransactionsFromSPAsync(int accountId)
    {
        var transactions = await _repo.GetByAccountIdFromSPAsync(accountId);
        return transactions;
    }
}
