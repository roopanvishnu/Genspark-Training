using System;
using Microsoft.EntityFrameworkCore;
using Notify.Models;

namespace Notify.Contexts;

public class NotifyContext : DbContext
{
    public NotifyContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<User> users { get; set; }
    public DbSet<Document> documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().HasOne(d => d.UploadedUser)
                                        .WithMany(u => u.Documents)
                                        .HasForeignKey(d => d.UploadedBy)
                                        .HasConstraintName("FK_Documents_Users")
                                        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

    }
}
