namespace Bank.Contexts;

using Bank.Models;
using Microsoft.EntityFrameworkCore;

public class BankContext : DbContext
{
    public BankContext(DbContextOptions<BankContext> options) : base(options)
    {
    }

    // DbSet for BankAccount model
    public DbSet<BankAccount> BankAccounts { get; set; }

    // DbSet for Transaction model
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BankAccount model configuration

        // Primary key for BankAccount is AccountNumber
        modelBuilder.Entity<BankAccount>()
            .HasKey(a => a.AccountNumber);

        // Enforces unique constraint on AccountNumber
        modelBuilder.Entity<BankAccount>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();

        // One-to-many relationship:
        // One BankAccount can have many Transactions
        modelBuilder.Entity<BankAccount>()
            .HasMany(a => a.Transactions)
            .WithOne(t => t.BankAccount)
            .HasForeignKey(t => t.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete: when a BankAccount is deleted, its Transactions are also deleted
    }
}