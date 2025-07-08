using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Commands;

public record BookAppointmentCommand(
    Guid ServiceId,
    Guid StaffId,
    Guid CustomerId, // This would be identified via bot interaction state
    DateTime StartTime
) : IRequest<AppointmentDto>;