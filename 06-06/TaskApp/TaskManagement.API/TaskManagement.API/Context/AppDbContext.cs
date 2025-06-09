
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(50);

            // TaskItem configuration
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskAttachment configuration
            modelBuilder.Entity<TaskAttachment>()
                .HasOne(a => a.TaskItem)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskItemId);

            // AuditLog optional JSON strings
            // AuditLog optional JSON strings
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.OldValues)
                .HasColumnType("jsonb");

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.NewValues)
                .HasColumnType("jsonb");


            // Soft delete global filter (optional, can be removed if handled manually)
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<TaskItem>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
