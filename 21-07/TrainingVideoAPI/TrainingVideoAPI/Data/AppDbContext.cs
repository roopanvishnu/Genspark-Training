using Microsoft.EntityFrameworkCore;

namespace TrainingVideoAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TrainingVideo> TrainingVideos { get; set; }
}

