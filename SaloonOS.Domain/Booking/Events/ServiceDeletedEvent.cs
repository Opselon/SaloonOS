using MediatR;

namespace SaloonOS.Domain.Booking.Events;

/// <summary>
/// A domain event that is published when a service is successfully deleted.
/// Since the service entity no longer exists, this event carries the necessary IDs
/// for listeners (like cache invalidators) to identify the affected tenant.
/// </summary>
public record ServiceDeletedEvent(Guid ServiceId, Guid ShopId) : INotification;