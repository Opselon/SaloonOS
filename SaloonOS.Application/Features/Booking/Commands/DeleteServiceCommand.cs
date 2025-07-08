using MediatR;

namespace SaloonOS.Application.Features.Booking.Commands;

public record DeleteServiceCommand(Guid ServiceId) : IRequest;