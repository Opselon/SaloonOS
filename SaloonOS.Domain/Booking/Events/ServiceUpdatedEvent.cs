using MediatR;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Domain.Booking.Events;

/// <summary>
/// A domain event that is published when an existing service is successfully updated and persisted.
/// It carries the updated Service aggregate root.
/// </summary>
public record ServiceUpdatedEvent(Service Service) : INotification;