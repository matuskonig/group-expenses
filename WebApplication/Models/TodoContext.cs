using Microsoft.EntityFrameworkCore;

namespace WebApplication.Models
{
    public sealed class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}