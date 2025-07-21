using AzureUserDev.model;
using Microsoft.EntityFrameworkCore;

namespace AzureUserDev.data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Person> Persons => Set<Person>();
}