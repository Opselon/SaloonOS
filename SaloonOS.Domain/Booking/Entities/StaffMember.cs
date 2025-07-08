using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

/// <summary>
/// Represents a staff member of a shop (e.g., barber, nail technician, stylist).
/// This entity is universal and can represent any service provider.
/// It belongs to a specific shop (tenant).
/// </summary>
public class StaffMember : BaseEntity
{
    public Guid ShopId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; } = true;

    // A staff member can be qualified to perform multiple services.
    // Inversely, a service can be performed by multiple staff members.
    // This creates a many-to-many relationship.
    // We will manage this relationship explicitly in the future.

    private StaffMember() { } // For EF Core

    public static StaffMember Create(Guid shopId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Staff member name cannot be empty.", nameof(name));
        }

        return new StaffMember
        {
            ShopId = shopId,
            Name = name
        };
    }
}