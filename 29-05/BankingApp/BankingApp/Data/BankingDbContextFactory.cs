using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.Data
{
    public class BankingDbContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();
            
            optionsBuilder.UseNpgsql("Host=localhost;Database=BankingDb;Username=roopanvishnu;Password=1234");

            return new BankingDbContext(optionsBuilder.Options);
        }
    }
}
