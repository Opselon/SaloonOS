// Path: SaloonOS.Domain/TenantManagement/Entities/Shop.cs
using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.TenantManagement.Entities;

public class Shop : BaseEntity
{
    public string Name { get; private set; }
    public long? OwnerTelegramUserId { get; private set; }
    public Guid BusinessCategoryId { get; private set; }
    public BusinessCategory BusinessCategory { get; private set; }
    public string HashedApiKey { get; private set; }
    public string DefaultLanguageCode { get; private set; }

    // --- ADD THIS PROPERTY ---
    public string PrimaryCurrencyCode { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private Shop() { } // For EF Core

    /// <summary>
    /// Updated factory method for creating a Shop. Now includes PrimaryCurrencyCode.
    /// </summary>
    public static Shop Create(
        string name,
        string hashedApiKey, // Note: a previous correction changed this to be pre-hashed
        string defaultLanguageCode,
        Guid businessCategoryId,
        string primaryCurrencyCode) // <-- ADD NEW PARAMETER
    {
        // Enforce invariants
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Shop name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(hashedApiKey)) throw new ArgumentException("API Key cannot be empty.", nameof(hashedApiKey));
        if (businessCategoryId == Guid.Empty) throw new ArgumentException("Business Category ID cannot be empty.", nameof(businessCategoryId));
        if (string.IsNullOrWhiteSpace(primaryCurrencyCode)) throw new ArgumentException("Primary currency code is required.", nameof(primaryCurrencyCode));

        var shop = new Shop
        {
            Name = name,
            HashedApiKey = hashedApiKey,
            DefaultLanguageCode = defaultLanguageCode,
            BusinessCategoryId = businessCategoryId,
            PrimaryCurrencyCode = primaryCurrencyCode, // <-- ASSIGN NEW PROPERTY
            CreatedAt = DateTime.UtcNow
        };

        return shop;
    }

    public void SetOwner(long telegramUserId)
    {
        OwnerTelegramUserId = telegramUserId;
    }
}