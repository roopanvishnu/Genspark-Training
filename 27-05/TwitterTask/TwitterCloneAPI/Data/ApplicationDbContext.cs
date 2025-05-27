using Microsoft.EntityFrameworkCore;
using TwitterCloneAPI.Models;

namespace TwitterCloneAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserFollow> UserFollows { get; set; } = null!;
        public DbSet<Tweet> Tweets { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Hashtag> Hashtags { get; set; } = null!;
        public DbSet<TweetHashtag> TweetHashtags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite PK for UserFollow join entity
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

            // Configure Follower relationship
            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Following relationship
            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite PK for TweetHashtag join entity
            modelBuilder.Entity<TweetHashtag>()
                .HasKey(th => new { th.TweetId, th.HashtagId });

            // Configure TweetHashtag relationships
            modelBuilder.Entity<TweetHashtag>()
                .HasOne(th => th.Tweet)
                .WithMany(t => t.TweetHashtags)
                .HasForeignKey(th => th.TweetId);

            modelBuilder.Entity<TweetHashtag>()
                .HasOne(th => th.Hashtag)
                .WithMany(h => h.TweetHashtags)
                .HasForeignKey(th => th.HashtagId);
        }
    }
}
