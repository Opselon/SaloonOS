// Path: SaloonOS.Application/Common/Contracts/IWorkScheduleRepository.cs
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface IWorkScheduleRepository : IRepository<WorkSchedule>
{
    /// <summary>
    /// Gets the schedule for a specific shop and/or staff member.
    /// </summary>
    Task<List<WorkSchedule>> GetSchedulesAsync(Guid shopId, Guid? staffId);

    /// <summary>
    /// Deletes all existing schedules for a given shop and/or staff member.
    /// This is used in the "upsert" logic.
    /// </summary>
    Task ClearSchedulesAsync(Guid shopId, Guid? staffId);
}