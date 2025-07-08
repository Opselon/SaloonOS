// Path: SaloonOS.Domain/Booking/Entities/WorkSchedule.cs
using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

public class WorkSchedule : BaseEntity
{
    public Guid ShopId { get; private set; }
    public Guid? StaffMemberId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    // Private constructor for EF Core.
    private WorkSchedule() { }

    /// <summary>
    /// Factory method to create a new, valid WorkSchedule.
    /// </summary>
    public static WorkSchedule Create(Guid shopId, Guid? staffMemberId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.");
        }

        return new WorkSchedule
        {
            ShopId = shopId,
            StaffMemberId = staffMemberId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime
        };
    }
}