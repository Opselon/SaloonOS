// Path: SaloonOS.Application/Features/Booking/Queries/ListStaffQuery.cs
using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Queries;

/// <summary>
/// Represents a query to get a list of all active staff members for the authenticated shop.
/// </summary>
public record ListStaffQuery : IRequest<IEnumerable<StaffMemberDto>>;