using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        // Convert the enum to a string in the database for readability.
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Price)
            .HasColumnType("decimal(10, 2)");

        // Indexing is crucial for performance of scheduling queries.
        // A composite index on ShopId, StaffMemberId, and StartTime is essential for finding conflicts quickly.
        builder.HasIndex(a => new { a.ShopId, a.StaffMemberId, a.StartTime, a.EndTime })
            .IsUnique(false); // Can't be unique as multiple staff can have same start time.

        builder.HasIndex(a => new { a.ShopId, a.StartTime, a.EndTime });
        builder.Property(a => a.RowVersion).IsRowVersion();
    }
}