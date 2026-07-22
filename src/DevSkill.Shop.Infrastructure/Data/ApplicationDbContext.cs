using DevSkill.Shop.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevSkill.Shop.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Category ──────────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.ImageName).HasMaxLength(500);
                entity.Property(c => c.IsActive).HasDefaultValue(true);
            });

            // ── Product ───────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(300);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Brand).HasMaxLength(200);
                entity.Property(p => p.Sku).HasMaxLength(100);
                entity.Property(p => p.Color).HasMaxLength(100);
                entity.Property(p => p.IsActive).HasDefaultValue(true);

                // Many Product → One Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ProductImage ──────────────────────────────────────────
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages");
                entity.HasKey(pi => pi.Id);
                entity.Property(pi => pi.Serial).IsRequired();
                entity.Property(pi => pi.ImageName).HasMaxLength(500);

                // Many ProductImage → One Product
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Stock ─────────────────────────────────────────────────
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stocks");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Quantity).IsRequired();
                entity.Property(s => s.LowStockQuantity).IsRequired();

                // One Stock ↔ One Product
                entity.HasOne(s => s.Product)
                      .WithOne(p => p.Stock)
                      .HasForeignKey<Stock>(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}