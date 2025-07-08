// Path: SaloonOS.Infrastructure/Persistence/Configurations/CustomerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        // A customer is unique per shop based on their Telegram User ID.
        // This composite index ensures we don't create duplicate customer records for the same user in the same shop.
        builder.HasIndex(c => new { c.ShopId, c.TelegramUserId }).IsUnique();

        builder.Property(c => c.FirstName).HasMaxLength(100);
        builder.Property(c => c.LastName).HasMaxLength(100);
        builder.Property(c => c.PreferredLanguageCode).IsRequired().HasMaxLength(10);
    }
}