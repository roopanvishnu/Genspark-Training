using BankingApp.DTOs;

namespace BankingApp.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto);
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
    Task<object?> GetTransactionsForAccountAsync(int accountId);
    Task<object?> GetTransactionsFromSPAsync(int accountId);
}