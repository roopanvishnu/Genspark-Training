using BankingApp.DTOs;
using BankingApp.Interfaces;
using BankingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionDto dto)
        {
            var transaction = await _service.CreateTransactionAsync(dto);
            return Ok(transaction);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetTransactionsForAccount(int accountId)
        {
            var transactions = await _service.GetTransactionsForAccountAsync(accountId);
            return Ok(transactions);
        }

        [HttpGet("sp/account/{accountId}")]
        public async Task<IActionResult> GetTransactionsFromStoredProcedure(int accountId)
        {
            var transactions = await _service.GetTransactionsFromSPAsync(accountId);
            return Ok(transactions);
        }
    }
}