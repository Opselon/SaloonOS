using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Queries;

public record GetAvailableTimeSlotsQuery(
    Guid ServiceId,
    Guid StaffId,
    DateTime Date
) : IRequest<IEnumerable<TimeSlotDto>>;