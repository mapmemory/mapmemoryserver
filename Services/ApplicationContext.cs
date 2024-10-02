using Microsoft.EntityFrameworkCore;
using Models;

namespace Services;

public class ApplicationContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<MapSpot> MapSpot { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .Property(u => u.Class)
        .HasConversion<string>();

        modelBuilder.Entity<MapSpot>()
        .Property(m => m.Type)
        .HasConversion<string>();
    }
}