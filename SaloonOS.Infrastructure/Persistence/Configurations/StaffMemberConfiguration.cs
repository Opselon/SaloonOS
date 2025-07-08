using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);

        // Each staff member must belong to one shop.
        builder.HasOne<Shop>()
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .IsRequired();

        // Index on ShopId for efficient lookups of a shop's staff.
        builder.HasIndex(s => s.ShopId);
    }
}