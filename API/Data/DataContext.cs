using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        //constructor
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // to avoid errors during migration

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserID, k.LikedUserId }); // creates primary key

            builder.Entity<UserLike>() // specifyiong relationship
                .HasOne(s => s.SourceUser)
                .WithMany(s => s.LikedUsers)
                .HasForeignKey(s => s.SourceUserID)
                .OnDelete(DeleteBehavior.Cascade); // when delet user, entities get deleted

            // other side of relationship
            builder.Entity<UserLike>() // specifyiong relationship
                .HasOne(s => s.LikedUser)
                .WithMany(s => s.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade); // when delet user, entities get deleted
        }
    }
}
