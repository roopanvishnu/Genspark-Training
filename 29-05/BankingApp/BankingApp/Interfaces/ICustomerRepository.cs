using BankingApp.Models;

namespace BankingApp.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
}