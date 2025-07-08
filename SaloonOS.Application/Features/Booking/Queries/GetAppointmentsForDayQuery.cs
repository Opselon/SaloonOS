// Path: SaloonOS.Application/Features/Booking/Queries/GetAppointmentsForDayQuery.cs
using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Queries;

/// <summary>
/// Represents an admin-level query to get the full schedule for a given day.
/// </summary>
public record GetAppointmentsForDayQuery(DateTime Date, long AdminTelegramUserId) : IRequest<IEnumerable<AdminAppointmentDto>>;