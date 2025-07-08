using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Booking.Entities;

/// <summary>
/// Represents the working hours for a specific day of the week.
/// This can be associated with an entire shop or a specific staff member.
/// </summary>
public class WorkSchedule : BaseEntity
{
    public Guid ShopId { get; private set; }

    // If StaffMemberId is null, this schedule applies to the whole shop.
    // If it has a value, it's an override for a specific staff member.
    public Guid? StaffMemberId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    // Store as TimeSpan for date-agnostic time representation.
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    private WorkSchedule() { }
}