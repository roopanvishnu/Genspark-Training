using BankingApp.DTOs;
using BankingApp.DTOs;
using BankingApp.Interfaces;
using BankingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(AccountDto dto)
        {
            var account = await _service.CreateAccountAsync(dto);
            return Ok(account);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _service.GetAllAccountsAsync();
            return Ok(accounts);
        }
    }
}