using MediatR;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Domain.Booking.Events;

/// <summary>
/// A domain event that is published when a new service is successfully created and persisted.
/// It carries the newly created Service aggregate root.
/// </summary>
public record ServiceCreatedEvent(Service Service) : INotification;