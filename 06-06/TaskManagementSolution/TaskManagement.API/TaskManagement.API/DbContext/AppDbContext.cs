using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models.Entities;

namespace TaskManagement.API.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
    public DbSet<Models.Entities.Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Models.Entities.Task>().ToTable("Tasks");
        
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<TaskAssignment>()
                    .HasOne(ta => ta.AsssignedToUser)
                    .WithMany(u => u.TaskAssignments)
                    .HasForeignKey(ta => ta.AsssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskAssignment>()
                    .HasOne(ta => ta.AssignedByUser)
                    .WithMany()
                    .HasForeignKey(ta => ta.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
        
    }
}