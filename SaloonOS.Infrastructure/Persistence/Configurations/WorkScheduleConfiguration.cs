// Path: SaloonOS.Infrastructure/Persistence/Configurations/WorkScheduleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Infrastructure.Persistence.Configurations;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.HasKey(ws => ws.Id);

        // A schedule is unique for a given shop, staff member, and day of the week.
        // The nullable StaffMemberId is handled correctly by the index.
        builder.HasIndex(ws => new { ws.ShopId, ws.StaffMemberId, ws.DayOfWeek }).IsUnique();
    }
}