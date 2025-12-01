using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class SalesContext : DbContext
{
    public DbSet<Sale> Sales { get; set; }
    
    public SalesContext(DbContextOptions<SalesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SaleProduct>()
            .HasKey(sp => new { sp.ProductId, sp.SaleId });
        
        modelBuilder.Entity<Sale>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);
    }
}