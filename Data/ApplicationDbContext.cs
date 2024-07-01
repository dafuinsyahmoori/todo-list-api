using Microsoft.EntityFrameworkCore;
using TodoList.Entities;

namespace TodoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(eb =>
            {
                eb.Property(u => u.FirstName).HasMaxLength(15).IsRequired();
                eb.Property(u => u.LastName).HasMaxLength(15).IsRequired();
                eb.Property(u => u.Email).HasMaxLength(50).IsRequired();
                eb.Property(u => u.Username).HasMaxLength(40).IsRequired();
                eb.Property(u => u.Password).HasMaxLength(100).IsRequired();

                eb.HasIndex(u => u.Email).IsUnique();
                eb.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Todo>(eb =>
            {
                eb.Property(t => t.Title).HasMaxLength(20).IsRequired();
                eb.Property(t => t.Description).HasColumnType("text");
            });
        }
    }
}