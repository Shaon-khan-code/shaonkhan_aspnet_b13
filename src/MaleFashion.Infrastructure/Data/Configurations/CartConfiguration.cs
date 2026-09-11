using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaleFashion.Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UserId);

        builder.HasIndex(c => c.GuestId)
            .IsUnique()
            .HasFilter("[GuestId] IS NOT NULL");
    }
}