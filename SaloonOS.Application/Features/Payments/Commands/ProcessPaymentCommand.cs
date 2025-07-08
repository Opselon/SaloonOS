// Path: SaloonOS.Application/Features/Payments/Commands/ProcessPaymentCommand.cs
using MediatR;
using SaloonOS.Domain.Payments.Entities;

namespace SaloonOS.Application.Features.Payments.Commands;

/// <summary>
/// A command to process a payment for a specific appointment.
/// This encapsulates the entire financial transaction flow.
/// </summary>
public record ProcessPaymentCommand(
    Guid AppointmentId,
    PaymentMethod Method,
    string? PaymentToken // Nullable, as it's not needed for 'Cash' payments
) : IRequest<string>; // Returns the provider transaction ID on success