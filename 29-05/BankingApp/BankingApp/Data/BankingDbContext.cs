using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data;

public class BankingDbContext:DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> opt) : base(opt)
    {
    }
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet <Transactions> Transactions => Set<Transactions>();
    public DbSet <Customer> Customers => Set<Customer>();
    public DbSet <CustomerAccount> CustomerAccounts => Set<CustomerAccount>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Branch>().HasMany(b => b.Accounts).WithOne(a => a.Branch).HasForeignKey(a => a.BranchId);
        
        mb.Entity<CustomerAccount>().HasKey(ca => new { ca.CustomerId, ca.AccountId });
        
        mb.Entity<CustomerAccount>().HasOne(ca => ca.Customer).WithMany(c => c.CustomerAccounts).HasForeignKey(ca => ca.CustomerId);
        
        mb.Entity<CustomerAccount>().HasOne(ca => ca.Account).WithMany(a => a.CustomerAccounts).HasForeignKey(ca => ca.AccountId);
        
        mb.Entity<Account>().HasMany(a => a.Transactions).WithOne(t =>t.Account!).HasForeignKey(t => t.AccountId);
    }
}