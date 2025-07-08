// Path: SaloonOS.Infrastructure/Persistence/Configurations/ShopConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.HashedApiKey).IsRequired().HasMaxLength(256);
        builder.Property(s => s.DefaultLanguageCode).IsRequired().HasMaxLength(10);

        // --- ADD THIS CONFIGURATION ---
        builder.Property(s => s.PrimaryCurrencyCode).IsRequired().HasMaxLength(3);

        builder.HasOne(s => s.BusinessCategory)
               .WithMany()
               .HasForeignKey(s => s.BusinessCategoryId)
               .IsRequired();

        builder.HasIndex(s => s.HashedApiKey).IsUnique();
        builder.HasIndex(s => s.BusinessCategoryId);
    }
}