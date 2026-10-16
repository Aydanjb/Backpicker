using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductWeight> ProductWeights => Set<ProductWeight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductWeight>()
            .HasKey(w => new { w.ProductId, w.Weight });

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Weights)
            .WithOne(w => w.Product)
            .HasForeignKey(w => w.ProductId);
    }
}
