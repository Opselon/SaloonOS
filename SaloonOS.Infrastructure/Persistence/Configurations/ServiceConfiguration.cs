using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Price)
            .HasColumnType("decimal(10, 2)");

        // Configure the one-to-many relationship with ServiceTranslation
        // When a Service is deleted, its translations should also be deleted.
        builder.HasMany(s => s.Translations)
            .WithOne()
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship to the Shop (Tenant)
        // A service must belong to a shop.
        builder.HasOne<Domain.TenantManagement.Entities.Shop>()
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .IsRequired();
    }
}