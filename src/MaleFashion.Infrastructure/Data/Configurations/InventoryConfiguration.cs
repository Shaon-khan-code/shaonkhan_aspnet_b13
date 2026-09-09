using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaleFashion.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.ProductId)
            .IsUnique();

        builder.Property(i => i.ReorderLevel)
            .HasDefaultValue(0);

        // AvailableQuantity and IsLowStock are computed, get-only properties
        // on the entity (no setter) — EF Core excludes them from the model
        // by convention, so no explicit .Ignore() is required here.
    }
}