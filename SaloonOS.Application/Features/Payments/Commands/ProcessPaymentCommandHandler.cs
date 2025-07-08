// Path: SaloonOS.Application/Features/Payments/Commands/ProcessPaymentCommandHandler.cs
using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Domain.Payments.Entities;

namespace SaloonOS.Application.Features.Payments.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly ITenantContext _tenantContext;

    public ProcessPaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _tenantContext = tenantContext;
    }

    public async Task<string> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId)
            ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        if (appointment.ShopId != shopId) throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Create the initial Payment record in a 'Pending' state.
        var payment = Payment.Create(
            shopId,
            appointment.Id,
            appointment.Price,
            appointment.Currency,
            request.Method);

        await _unitOfWork.GetRepository<Payment>().AddAsync(payment);
        await _unitOfWork.CompleteAsync(); // Save the pending payment first.

        string providerTransactionId = $"CASH-{payment.Id}"; // Default for cash payments

        try
        {
            if (request.Method == PaymentMethod.Card)
            {
                if (string.IsNullOrWhiteSpace(request.PaymentToken))
                {
                    throw new ArgumentException("A payment token is required for card payments.");
                }
                // Process the payment through the external provider (Stripe).
                providerTransactionId = await _paymentService.ProcessPaymentAsync(
                    payment.Amount, payment.Currency, request.PaymentToken, $"Payment for appointment {appointment.Id}");
            }

            // If we reach here, the payment was successful (or it was cash).
            payment.MarkAsSucceeded(providerTransactionId);
        }
        catch (Exception)
        {
            payment.MarkAsFailed();
            // Re-throw the original exception after marking our internal record as failed.
            throw;
        }
        finally
        {
            // IMPORTANT: Save the final state of the payment (Succeeded or Failed).
            await _unitOfWork.CompleteAsync();
        }

        // TODO: Publish PaymentSucceededEvent

        return providerTransactionId;
    }
}