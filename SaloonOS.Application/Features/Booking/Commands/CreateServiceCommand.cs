using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Commands;

public record CreateServiceCommand(
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int DurationInMinutes
) : IRequest<ServiceDto>;