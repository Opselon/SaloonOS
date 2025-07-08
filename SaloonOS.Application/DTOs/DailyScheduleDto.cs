// Path: SaloonOS.Application/DTOs/DailyScheduleDto.cs
namespace SaloonOS.Application.DTOs;

/// <summary>
/// A DTO representing the schedule for a single day of the week.
/// </summary>
public class DailyScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}