using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

public enum AppointmentStatus { Scheduled, Completed, CancelledByCustomer, CancelledByShop, NoShow }

/// <summary>
/// The Appointment Aggregate Root. This is the central entity for the booking context.
/// It represents a contract between a customer and the shop for a specific service at a specific time.
/// All business logic related to an appointment's lifecycle should be encapsulated here.
/// </summary>
public class Appointment : BaseEntity
{
    public Guid ShopId { get; private set; }
    public Guid CustomerId { get; private set; } // The customer who booked
    public Guid ServiceId { get; private set; } // The service being provided
    public Guid StaffMemberId { get; private set; } // The staff member providing the service
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }

    private Appointment() { } // For EF Core

    /// <summary>
    /// Factory method for creating a new, valid Appointment.
    /// This is the only way an Appointment should be instantiated.
    /// </summary>
    public static Appointment Create(
        Guid shopId,
        Guid customerId,
        Guid serviceId,
        Guid staffMemberId,
        DateTime startTime,
        int durationInMinutes,
        decimal price,
        string currency)
    {
        if (durationInMinutes <= 0) throw new ArgumentException("Service duration must be positive.", nameof(durationInMinutes));
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (customerId == Guid.Empty) throw new ArgumentException("A valid customer must be provided.", nameof(customerId));

        return new Appointment
        {
            ShopId = shopId,
            CustomerId = customerId,
            ServiceId = serviceId,
            StaffMemberId = staffMemberId,
            StartTime = startTime.ToUniversalTime(), // Always store in UTC
            EndTime = startTime.ToUniversalTime().AddMinutes(durationInMinutes),
            Price = price,
            Currency = currency,
            Status = AppointmentStatus.Scheduled
        };
    }

    /// <summary>
    /// Encapsulates the business logic for cancelling an appointment.
    /// </summary>
    public void Cancel(bool byShop = false)
    {
        if (Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment.");
        }
        if (StartTime < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot cancel an appointment that is in the past.");
        }
        Status = byShop ? AppointmentStatus.CancelledByShop : AppointmentStatus.CancelledByCustomer;
    }

    /// <summary>
    /// Encapsulates the business logic for completing an appointment.
    /// </summary>
    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException("Only a scheduled appointment can be marked as completed.");
        }
        Status = AppointmentStatus.Completed;
    }
}