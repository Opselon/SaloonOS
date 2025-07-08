using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(SaloonOSDbContext context) : base(context) { }

    public async Task<List<Appointment>> GetAppointmentsForDay(Guid shopId, Guid staffId, DateTime date)
    {
        var startOfDay = date.Date.ToUniversalTime();
        var endOfDay = startOfDay.AddDays(1);

        return await _context.Appointments
            .Where(a => a.ShopId == shopId &&
                        a.StaffMemberId == staffId &&
                        a.StartTime >= startOfDay &&
                        a.StartTime < endOfDay &&
                        a.Status == AppointmentStatus.Scheduled)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }
    public async Task<List<Appointment>> GetUpcomingAppointmentsForCustomerAsync(Guid shopId, Guid customerId)
    {
        var now = DateTime.UtcNow;
        return await _context.Appointments
            .Where(a =>
                a.ShopId == shopId &&
                a.CustomerId == customerId &&
                a.Status == AppointmentStatus.Scheduled &&
                a.StartTime > now)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }
    public async Task<bool> IsSlotAvailable(Guid shopId, Guid staffId, DateTime proposedStartTime, int durationInMinutes)
    {
        var proposedUtcStart = proposedStartTime.ToUniversalTime();
        var proposedUtcEnd = proposedUtcStart.AddMinutes(durationInMinutes);

        // Check for any existing appointments that overlap with the proposed time slot.
        // An overlap occurs if:
        // (ExistingStart < ProposedEnd) AND (ExistingEnd > ProposedStart)
        var hasConflict = await _context.Appointments
            .AnyAsync(a =>
                a.ShopId == shopId &&
                a.StaffMemberId == staffId &&
                a.Status == AppointmentStatus.Scheduled &&
                a.StartTime < proposedUtcEnd && a.EndTime > proposedUtcStart);

        return !hasConflict;
    }
}