using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.HashedApiKey)
            .IsRequired()
            .HasMaxLength(256);

        // This creates an index on the API key for fast lookups.
        builder.HasIndex(s => s.HashedApiKey).IsUnique();
    }
}