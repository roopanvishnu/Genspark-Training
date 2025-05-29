using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Repositories;

public class CustomerRepository:ICustomerRepository
{
    private readonly BankingDbContext _context;

    public CustomerRepository(BankingDbContext context)
    {
        _context = context;
    }
    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() => await Task.FromResult<IEnumerable<Customer>>(_context.Customers.ToList());

    public async Task<Customer?> GetByIdAsync(int id) => await _context.Customers.FindAsync(id).AsTask();
}