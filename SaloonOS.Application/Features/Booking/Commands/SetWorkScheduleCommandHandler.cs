// Path: SaloonOS.Application/Features/Booking/Commands/SetWorkScheduleCommandHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Commands;

public class SetWorkScheduleCommandHandler : IRequestHandler<SetWorkScheduleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public SetWorkScheduleCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task Handle(SetWorkScheduleCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();
        // ... Add authorization check for AdminTelegramUserId ...

        // "UPSERT" LOGIC
        // The IUnitOfWork.WorkSchedules property now exists and will compile.
        await _unitOfWork.WorkSchedules.ClearSchedulesAsync(shopId, request.StaffMemberId);

        foreach (var dailySchedule in request.DailySchedules)
        {
            // Use the factory method, which resolves the "inaccessible" error.
            var newSchedule = WorkSchedule.Create(
                shopId,
                request.StaffMemberId,
                dailySchedule.DayOfWeek,
                dailySchedule.StartTime,
                dailySchedule.EndTime);

            await _unitOfWork.GetRepository<WorkSchedule>().AddAsync(newSchedule);
        }

        await _unitOfWork.CompleteAsync();
    }
}