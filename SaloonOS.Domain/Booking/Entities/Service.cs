using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

public class Service : BaseEntity
{
    public Guid ShopId { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }
    public int DurationInMinutes { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property for the translations
    public ICollection<ServiceTranslation> Translations { get; private set; } = new List<ServiceTranslation>();

    // Private constructor for EF Core
    private Service() { }
}

// This is part of the Service aggregate, so it lives in the same file or a nearby one.
public class ServiceTranslation : BaseEntity
{
    public Guid ServiceId { get; private set; }
    public string LanguageCode { get; private set; } // e.g., "en-US", "fa-IR", "ru-RU"
    public string Name { get; private set; }
    public string? Description { get; private set; }
}