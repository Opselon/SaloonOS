using MediatR;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// Represents a command to cancel a specific appointment.
/// Carries the necessary identifiers to find and authorize the cancellation.
/// </summary>
public record CancelAppointmentCommand(Guid AppointmentId, Guid CustomerId) : IRequest;