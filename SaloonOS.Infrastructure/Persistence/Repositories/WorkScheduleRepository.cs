// Path: SaloonOS.Infrastructure/Persistence/Repositories/WorkScheduleRepository.cs
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

public class WorkScheduleRepository : Repository<WorkSchedule>, IWorkScheduleRepository
{
    public WorkScheduleRepository(SaloonOSDbContext context) : base(context) { }

    public async Task<List<WorkSchedule>> GetSchedulesAsync(Guid shopId, Guid? staffId)
    {
        return await _context.WorkSchedules
            .Where(ws => ws.ShopId == shopId && ws.StaffMemberId == staffId)
            .ToListAsync();
    }

    public async Task ClearSchedulesAsync(Guid shopId, Guid? staffId)
    {
        await _context.WorkSchedules
            .Where(ws => ws.ShopId == shopId && ws.StaffMemberId == staffId)
            .ExecuteDeleteAsync(); // EF Core 7+ bulk delete
    }
}