// Path: SaloonOS.Application/Features/Booking/Commands/CancelAppointmentByAdminCommand.cs
using MediatR;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// Admin command to cancel a specific appointment.
/// </summary>
public record CancelAppointmentByAdminCommand(Guid AppointmentId, long AdminTelegramUserId) : IRequest;