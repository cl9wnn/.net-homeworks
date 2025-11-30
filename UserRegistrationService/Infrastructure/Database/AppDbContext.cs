using Core.Entities;
using Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options ) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}