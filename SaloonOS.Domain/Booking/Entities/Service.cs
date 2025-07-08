using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

public class Service : BaseEntity
{
    public Guid ShopId { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }
    public int DurationInMinutes { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Use a private backing field for the collection to enforce control.
    private readonly List<ServiceTranslation> _translations = new();
    public IReadOnlyCollection<ServiceTranslation> Translations => _translations.AsReadOnly();

    /// <summary>
    /// The private constructor is crucial for DDD. It forces consumers to use the static Create factory method,
    /// ensuring that no Service entity can ever be instantiated in an invalid state.
    /// EF Core can use this private constructor.
    /// </summary>
    private Service() { }

    /// <summary>
    /// The official factory method for creating a new Service. This is the sole entry point for creation.
    /// It enforces invariants (business rules) before creating the object.
    /// </summary>
    public static Service Create(
        Guid shopId,
        decimal price,
        string currency,
        int durationInMinutes,
        string initialLanguageCode,
        string initialName,
        string? initialDescription)
    {
        // --- Enforce Invariants (Guard Clauses) ---
        if (price <= 0) throw new ArgumentException("Price must be positive.", nameof(price));
        if (durationInMinutes <= 0) throw new ArgumentException("Duration must be positive.", nameof(durationInMinutes));
        if (string.IsNullOrWhiteSpace(initialName)) throw new ArgumentException("Initial service name cannot be empty.", nameof(initialName));

        var service = new Service
        {
            ShopId = shopId,
            Price = price,
            Currency = currency,
            DurationInMinutes = durationInMinutes
        };

        // Automatically create the first translation, ensuring a service is never without a name.
        service.AddOrUpdateTranslation(initialLanguageCode, initialName, initialDescription);

        return service;
    }

    /// <summary>
    /// A controlled method to add or update a translation for this service.
    /// This keeps the logic of managing translations inside the Aggregate Root.
    /// </summary>
    public void AddOrUpdateTranslation(string languageCode, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Service name cannot be empty.", nameof(name));

        var existingTranslation = _translations.FirstOrDefault(t => t.LanguageCode == languageCode);
        if (existingTranslation != null)
        {
            existingTranslation.UpdateDetails(name, description);
        }
        else
        {
            _translations.Add(ServiceTranslation.Create(this.Id, languageCode, name, description));
        }
    }
}


// This entity is part of the Service aggregate.
public class ServiceTranslation : BaseEntity
{
    public Guid ServiceId { get; private set; }
    public string LanguageCode { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Make the constructor internal so it can only be created from within the Domain assembly,
    // specifically by the Service aggregate root.
    internal ServiceTranslation() { }

    internal static ServiceTranslation Create(Guid serviceId, string languageCode, string name, string? description)
    {
        return new ServiceTranslation
        {
            ServiceId = serviceId,
            LanguageCode = languageCode,
            Name = name,
            Description = description
        };
    }

    internal void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}