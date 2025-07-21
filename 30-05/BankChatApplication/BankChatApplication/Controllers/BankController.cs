using Bank.Contexts;
using Bank.Interfaces;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/bank")]
public class BankController : ControllerBase
{
    private readonly IBankService _bankService;

    public BankController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpPost("account")]
    public async Task<IActionResult> CreateAccount([FromBody] BankAccount account)
    {
        var createdAccount = await _bankService.CreateAccountAsync(account);
        return CreatedAtAction(nameof(GetAccountBalance), new { accountNumber = createdAccount.AccountNumber }, createdAccount);
    }

    [HttpPost("account/deposit")]
    public async Task<IActionResult> DepositToAccount([FromBody] Transaction transaction)
    {
        var result = await _bankService.DepositAsync(transaction.BankAccountId, transaction.Amount);
        if (result)
            return Ok("Deposit successful");
        return BadRequest("Deposit failed");
    }

    [HttpPost("account/withdraw")]
    public async Task<IActionResult> WithdrawFromAccount([FromBody] Transaction transaction)
    {
        var result = await _bankService.WithdrawAsync(transaction.BankAccountId, transaction.Amount);
        if (result)
            return Ok("Withdrawal successful");
        return BadRequest("Withdrawal failed");
    }

    [HttpGet("account/{accountNumber}/balance")]
    public async Task<IActionResult> GetAccountBalance(string accountNumber)
    {
        var balance = await _bankService.GetBalanceAsync(accountNumber);
        return Ok(balance);
    }
}