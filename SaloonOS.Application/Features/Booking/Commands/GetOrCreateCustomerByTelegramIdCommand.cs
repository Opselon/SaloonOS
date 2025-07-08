// Path: SaloonOS.Application/Features/Booking/Commands/GetOrCreateCustomerByTelegramIdCommand.cs
using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.Booking.Commands;

/// <summary>
/// A command that encapsulates the logic to either retrieve an existing customer
/// or create a new one based on their Telegram ID. This is an idempotent operation.
/// </summary>
public record GetOrCreateCustomerByTelegramIdCommand(
    long TelegramUserId,
    string? FirstName,
    string? LastName,
    string DetectedLanguageCode // The language code detected by Telegram
) : IRequest<CustomerDto>;