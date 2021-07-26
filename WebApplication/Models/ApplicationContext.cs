using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public sealed class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FriendRequest>(entity =>
            {
                entity.HasOne(friendRequest => friendRequest.To)
                    .WithMany(u => u.IncomingRequests)
                    .IsRequired();
                entity.HasOne(friendRequest => friendRequest.From)
                    .WithMany(u => u.SentRequests)
                    .IsRequired();
            });
            base.OnModelCreating(builder);
        }
    }
}