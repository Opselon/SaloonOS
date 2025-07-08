// Path: SaloonOS.Application/Features/Booking/Commands/CancelAppointmentByAdminCommandHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;
using System.Data;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CancelAppointmentByAdminCommandHandler : IRequestHandler<CancelAppointmentByAdminCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    // Inject IPublisher here to publish AppointmentCancelledEvent

    public CancelAppointmentByAdminCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task Handle(CancelAppointmentByAdminCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // Authorization... (same as CompleteAppointmentCommandHandler)
        var shop = await _unitOfWork.Shops.GetByIdAsync(shopId) ?? throw new InvalidOperationException();
        if (shop.OwnerTelegramUserId != request.AdminTelegramUserId) throw new UnauthorizedAccessException("AdminNotAuthorized");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId)
            ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        if (appointment.ShopId != shopId) throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Domain Logic
        appointment.Cancel(byShop: true);

        // Persistence with Concurrency Check
        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("ConcurrencyConflict");
        }

        // TODO: Publish AppointmentCancelledEvent
    }
}