using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductWeight> ProductWeights => Set<ProductWeight>();
    public DbSet<Retailer> Retailers => Set<Retailer>();
    public DbSet<RetailerProduct> RetailerProducts => Set<RetailerProduct>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductWeight>()
            .HasKey(w => new { w.ProductId, w.Size });

        modelBuilder.Entity<RetailerProduct>()
            .HasIndex(rp => new { rp.RetailerId, rp.ProductUrl })
            .IsUnique();

        modelBuilder.Entity<Retailer>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.MatchKey)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Weights)
            .WithOne(w => w.Product)
            .HasForeignKey(w => w.ProductId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.RetailerProducts)
            .WithOne(rp => rp.Product)
            .HasForeignKey(rp => rp.ProductId);

        modelBuilder.Entity<Retailer>()
            .HasMany(r => r.RetailerProducts)
            .WithOne(rp => rp.Retailer)
            .HasForeignKey(r => r.RetailerId);
    }
}