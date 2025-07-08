using MediatR;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Domain.Booking.Events;

/// <summary>
/// A domain event published when an appointment is successfully cancelled.
/// Carries the cancelled appointment details for listeners (e.g., notification services).
/// </summary>
public record AppointmentCancelledEvent(Appointment CancelledAppointment) : INotification;