using BankingApp.DTOs;
using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer { FullName = dto.FullName };
        customer = await _repo.AddAsync(customer);
        return new CustomerDto { Id = customer.Id, FullName = customer.FullName };
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _repo.GetAllAsync();
        return customers.Select(c => new CustomerDto { Id = c.Id, FullName = c.FullName });
    }
}
