namespace Bank.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Bank.Contexts;

public class BankContextFactory : IDesignTimeDbContextFactory<BankContext>
{
    public BankContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankContext>();
        
        // Replace with your actual connection string
        var connectionString = "Host=localhost;Port=5432;Database=bankchat;Username=roopanvishnu;Password=1234";
        optionsBuilder.UseNpgsql(connectionString);

        return new BankContext(optionsBuilder.Options);
    }
}
