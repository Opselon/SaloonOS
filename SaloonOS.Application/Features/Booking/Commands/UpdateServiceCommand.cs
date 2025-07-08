using MediatR;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// Command to update an existing service. It includes all editable fields.
/// The ServiceId is passed via the route, and the rest in the body.
/// </summary>
public record UpdateServiceCommand(
    Guid ServiceId,
    string Name,
    string? Description,
    decimal Price,
    int DurationInMinutes
) : IRequest; // This command doesn't need to return data, a 204 NoContent is sufficient.