// Path: SaloonOS.Application/Features/Booking/Commands/SetWorkScheduleCommand.cs
using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// Command to set the entire weekly schedule for a shop or a specific staff member.
/// </summary>
public record SetWorkScheduleCommand(
    long AdminTelegramUserId,
    Guid? StaffMemberId, // Null for the main shop schedule
    List<DailyScheduleDto> DailySchedules
) : IRequest;