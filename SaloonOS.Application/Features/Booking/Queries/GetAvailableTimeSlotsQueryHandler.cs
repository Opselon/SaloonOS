// Path: SaloonOS.Application/Features/Booking/Queries/GetAvailableTimeSlotsQueryHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Queries;

public class GetAvailableTimeSlotsQueryHandler : IRequestHandler<GetAvailableTimeSlotsQuery, IEnumerable<TimeSlotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetAvailableTimeSlotsQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();
        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException(nameof(Service), request.ServiceId);

        // Fetch Work Schedule logic is now correct.
        var schedule = (await _unitOfWork.WorkSchedules.GetSchedulesAsync(shopId, request.StaffId))
                       .FirstOrDefault(ws => ws.DayOfWeek == request.Date.DayOfWeek);
        if (schedule is null)
        {
            schedule = (await _unitOfWork.WorkSchedules.GetSchedulesAsync(shopId, null))
                       .FirstOrDefault(ws => ws.DayOfWeek == request.Date.DayOfWeek);
        }
        if (schedule is null)
        {
            return Enumerable.Empty<TimeSlotDto>();
        }

        var workStart = request.Date.Date.Add(schedule.StartTime);
        var workEnd = request.Date.Date.Add(schedule.EndTime);

        // --- COMPLETING THE ALGORITHM ---
        // The previous version was missing this entire block.
        var existingAppointments = await _unitOfWork.Appointments.GetAppointmentsForDay(shopId, request.StaffId, request.Date);
        var availableSlots = new List<TimeSlotDto>();
        var potentialSlot = workStart;
        const int slotIntervalMinutes = 15; // The "step" for checking availability.

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

        return availableSlots; // This now returns a value on all code paths.
    }
}