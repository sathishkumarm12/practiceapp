using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserID });
            builder.Entity<UserLike>()
                    .HasOne(s => s.SourceUser)
                    .WithMany(d => d.LikedUsers)
                    .HasForeignKey(k => k.SourceUserId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                    .HasOne(s => s.LikedUser)
                    .WithMany(d => d.LikedByUsers)
                    .HasForeignKey(k => k.LikedUserID)
                    .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Message>()
                    .HasOne(u => u.Recipient)
                    .WithMany(m => m.MessagesRecieved)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne(u => u.Sender)
                   .WithMany(m => m.MessagesSent)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}