using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Queries;

public class GetAvailableTimeSlotsQueryHandler : IRequestHandler<GetAvailableTimeSlotsQuery, IEnumerable<TimeSlotDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailableTimeSlotsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException(nameof(Service), request.ServiceId);

        // 1. Fetch Work Schedule
        // In a real system, you would fetch the specific schedule for the day.
        // We'll hardcode a 9 AM to 5 PM schedule for this implementation.
        var workStart = request.Date.Date.AddHours(9);
        var workEnd = request.Date.Date.AddHours(17);

        // 2. Fetch Existing Appointments
        var existingAppointments = await _unitOfWork.Appointments.GetAppointmentsForDay(
            service.ShopId, request.StaffId, request.Date);

        // 3. Generate Available Slots
        var availableSlots = new List<TimeSlotDto>();
        var potentialSlot = workStart;
        const int slotIntervalMinutes = 15;

        while (potentialSlot.AddMinutes(service.DurationInMinutes) <= workEnd)
        {
            var potentialSlotEnd = potentialSlot.AddMinutes(service.DurationInMinutes);
            bool isOverlapping = existingAppointments.Any(a =>
                a.StartTime < potentialSlotEnd && a.EndTime > potentialSlot);

            if (!isOverlapping)
            {
                availableSlots.Add(new TimeSlotDto { StartTime = potentialSlot });
            }

            potentialSlot = potentialSlot.AddMinutes(slotIntervalMinutes);
        }

        return availableSlots;
    }
}