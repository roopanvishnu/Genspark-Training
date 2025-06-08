using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Context;

public class AppDbContext:DbContext
{
    public DbSet<AppUser> Users { get; set; } 
    public DbSet<WorkItem> WorkItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Table name
        modelBuilder.Entity<AppUser>().ToTable("Users");
        modelBuilder.Entity<WorkItem>().ToTable("WorkItems");
        
        //primary keys
        modelBuilder.Entity<AppUser>().HasKey(u => u.Id);
        modelBuilder.Entity<WorkItem>().HasKey(w =>w.WorkItemId);
        
        // Enum to string Conversion
        modelBuilder.Entity<AppUser>().Property(u =>u.Role).HasConversion<string>();
        modelBuilder.Entity<WorkItem>().Property(w =>w.WorkStatus).HasConversion<string>();
        
        //Relationships
        
        modelBuilder.Entity<WorkItem>()
                    .HasOne(w => w.AssignedByUser)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(w => w.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<WorkItem>()
                    .HasOne(w =>w.AssignedToUser)
                    .WithMany(u =>u.TasksAssignedToMe)
                    .HasForeignKey(w => w.AssignedToUserId)
                    .OnDelete(DeleteBehavior.SetNull);
        
        base.OnModelCreating(modelBuilder);
    }
}