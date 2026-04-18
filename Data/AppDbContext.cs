using DemoManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(user => user.Username)
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(user => user.Password)
            .HasMaxLength(100);

        modelBuilder.Entity<Product>()
            .Property(product => product.Name)
            .HasMaxLength(150);

        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasColumnType("decimal(18,2)");
    }
}
