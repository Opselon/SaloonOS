// Path: SaloonOS.Domain/Booking/Entities/Customer.cs
using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

/// <summary>
/// The Customer Aggregate Root. Represents a unique customer for a specific shop.
/// A physical person is represented by a unique Customer record for each Shop they interact with.
/// This allows for different preferences or details per shop.
/// </summary>
public class Customer : BaseEntity
{
    public Guid ShopId { get; private set; }
    public long TelegramUserId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string PreferredLanguageCode { get; private set; } = string.Empty; // e.g., "en-US", "fa-IR"
    public DateTime FirstInteractionAt { get; private set; }

    private Customer() { } // For EF Core

    public static Customer Create(
        Guid shopId,
        long telegramUserId,
        string? firstName,
        string? lastName,
        string preferredLanguageCode)
    {
        if (telegramUserId <= 0) throw new ArgumentException("Telegram User ID must be a positive number.", nameof(telegramUserId));
        if (string.IsNullOrWhiteSpace(preferredLanguageCode)) throw new ArgumentException("A preferred language code must be provided.", nameof(preferredLanguageCode));

        return new Customer
        {
            ShopId = shopId,
            TelegramUserId = telegramUserId,
            FirstName = firstName,
            LastName = lastName,
            PreferredLanguageCode = preferredLanguageCode,
            FirstInteractionAt = DateTime.UtcNow
        };
    }

    public void UpdateLanguage(string newLanguageCode)
    {
        if (!string.IsNullOrWhiteSpace(newLanguageCode))
        {
            PreferredLanguageCode = newLanguageCode;
        }
    }
}