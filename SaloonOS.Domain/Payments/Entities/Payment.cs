// Path: SaloonOS.Domain/Payments/Entities/Payment.cs
using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Payments.Entities;

public enum PaymentStatus { Pending, Succeeded, Failed }
public enum PaymentMethod { Card, Cash, Other }

/// <summary>
/// The Payment Aggregate Root. Represents a single financial transaction.
/// It is linked to a specific appointment and contains details from the payment provider.
/// </summary>
public class Payment : BaseEntity
{
    public Guid ShopId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }

    // The unique ID from the external payment provider (e.g., Stripe Charge ID).
    public string? ProviderTransactionId { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private Payment() { }

    public static Payment Create(
        Guid shopId,
        Guid appointmentId,
        decimal amount,
        string currency,
        PaymentMethod method)
    {
        // Invariants
        if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));

        return new Payment
        {
            ShopId = shopId,
            AppointmentId = appointmentId,
            Amount = amount,
            Currency = currency,
            Method = method,
            Status = PaymentStatus.Pending // Initially pending until processed
        };
    }

    public void MarkAsSucceeded(string providerTransactionId)
    {
        Status = PaymentStatus.Succeeded;
        ProviderTransactionId = providerTransactionId;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}