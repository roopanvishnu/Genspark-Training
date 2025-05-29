using BankingApp.DTOs;

namespace BankingApp.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customerDto);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
}