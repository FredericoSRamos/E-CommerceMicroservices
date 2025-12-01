using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class UsersContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.OwnsOne(u => u.Address);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}