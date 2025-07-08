// Path: SaloonOS.Application/Features/Booking/Commands/CompleteAppointmentCommand.cs
using MediatR;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// Admin command to mark a specific appointment as completed.
/// </summary>
public record CompleteAppointmentCommand(Guid AppointmentId, long AdminTelegramUserId) : IRequest;