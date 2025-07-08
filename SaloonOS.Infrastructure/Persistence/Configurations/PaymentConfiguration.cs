// Path: SaloonOS.Infrastructure/Persistence/Configurations/PaymentConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Payments.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(10, 2)");
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(50);

        // A payment is uniquely associated with one appointment.
        builder.HasIndex(p => p.AppointmentId).IsUnique();

        builder.Property(p => p.ProviderTransactionId).HasMaxLength(255);
        builder.HasIndex(p => p.ProviderTransactionId);
    }
}