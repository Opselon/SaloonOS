// Path: SaloonOS.Domain/TenantManagement/Entities/Shop.cs
using SaloonOS.Domain.Common;
using SaloonOS.Domain.TenantManagement.Entities;

public class Shop : BaseEntity
{
    public string Name { get; private set; }
    public long? OwnerTelegramUserId { get; private set; } // <-- ADD THIS, make it nullable for other admin types
    public Guid BusinessCategoryId { get; private set; }
    public BusinessCategory BusinessCategory { get; private set; }
    public string HashedApiKey { get; private set; }
    public string DefaultLanguageCode { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Shop() { }

    /// <summary>
    /// Factory method for creating a Shop. Now includes BusinessCategoryId.
    /// </summary>
    public static Shop Create(string name, string apiKey, string defaultLanguageCode, Guid businessCategoryId)
    {
        // Enforce invariants
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Shop name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("API Key cannot be empty.", nameof(apiKey));
        if (businessCategoryId == Guid.Empty) throw new ArgumentException("Business Category ID cannot be empty.", nameof(businessCategoryId));

        var shop = new Shop
        {
            Name = name,
            HashedApiKey = HashApiKey(apiKey), // Use the static helper for hashing
            DefaultLanguageCode = defaultLanguageCode,
            BusinessCategoryId = businessCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        // We don't set the navigation property here because the BusinessCategory
        // object itself isn't passed to the factory. EF Core will populate it
        // based on the BusinessCategoryId when the object is loaded from the DB.

        return shop;
    }
    public void SetOwner(long telegramUserId)
    {
        OwnerTelegramUserId = telegramUserId;
    }
    // Static helper for hashing API keys - must be consistent across handlers.
    private static string HashApiKey(string apiKey)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}