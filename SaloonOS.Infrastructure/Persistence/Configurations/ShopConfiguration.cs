// ...
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.HashedApiKey).IsRequired().HasMaxLength(256);
        builder.Property(s => s.DefaultLanguageCode).IsRequired().HasMaxLength(10);

        // Configure the relationship with BusinessCategory
        builder.HasOne(s => s.BusinessCategory) // Navigation Property
               .WithMany() // Assuming BusinessCategory doesn't have a collection of Shops back
               .HasForeignKey(s => s.BusinessCategoryId) // Foreign Key
               .IsRequired();

        builder.HasIndex(s => s.HashedApiKey).IsUnique();
        builder.HasIndex(s => s.BusinessCategoryId); // Index for filtering by category
    }
}