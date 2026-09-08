using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaleFashion.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        // Explicit 1:1 configuration — EF Core cannot reliably infer
        // principal/dependent for one-to-one relationships by convention.
        builder.HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enforces true one-to-one at the database level: a product
        // cannot have more than one inventory record.
        builder.HasIndex(i => i.ProductId)
            .IsUnique();
    }
}