using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Models
{
    public sealed class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<FriendshipStatus> FriendRequests { get; set; }
        public DbSet<SinglePurposeUserGroup> UserGroups { get; set; }
        public DbSet<UnidirectionalPaymentGroup> PaymentGroups { get; set; }
        public DbSet<SinglePayment> SinglePayments { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FriendshipStatus>(entity =>
            {
                entity.HasOne(friendRequest => friendRequest.To)
                    .WithMany(u => u.IncomingRequests)
                    .IsRequired();
                entity.HasOne(friendRequest => friendRequest.From)
                    .WithMany(u => u.SentRequests)
                    .IsRequired();
            });
        }
    }
}