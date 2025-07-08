using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Events;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IPublisher _publisher;

    public CancelAppointmentCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _publisher = publisher;
    }

    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId);

        // --- AUTHORIZATION & VALIDATION ---
        // Ensure the appointment exists, belongs to the correct shop, and belongs to the customer making the request.
        if (appointment is null || appointment.ShopId != shopId || appointment.CustomerId != request.CustomerId)
        {
            throw new NotFoundException("Appointment", request.AppointmentId);
        }

        // --- DOMAIN LOGIC ---
        // Delegate the cancellation logic to the domain entity itself.
        try
        {
            appointment.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            // The domain model threw an exception (e.g., trying to cancel a completed appointment).
            // We re-throw this as our own specific exception type or message for the API layer.
            throw new InvalidOperationException("CannotCancelAppointment", ex);
        }

        // --- PERSISTENCE ---
        await _unitOfWork.CompleteAsync();

        // --- PUBLISH EVENT ---
        await _publisher.Publish(new AppointmentCancelledEvent(appointment), cancellationToken);
    }
}