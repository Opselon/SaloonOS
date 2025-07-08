using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Queries;

/// <summary>
/// Represents a query to get all upcoming appointments for the specified customer.
/// </summary>
public record GetCustomerAppointmentsQuery(Guid CustomerId) : IRequest<IEnumerable<AppointmentDto>>;