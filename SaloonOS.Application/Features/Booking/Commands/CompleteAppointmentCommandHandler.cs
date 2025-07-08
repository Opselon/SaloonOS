// Path: SaloonOS.Application/Features/Booking/Commands/CompleteAppointmentCommandHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;
using System.Data;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CompleteAppointmentCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // --- AUTHORIZATION ---
        var shop = await _unitOfWork.Shops.GetByIdAsync(shopId) ?? throw new InvalidOperationException("Shop context not found.");
        if (shop.OwnerTelegramUserId != request.AdminTelegramUserId) throw new UnauthorizedAccessException("AdminNotAuthorized");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId)
            ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        if (appointment.ShopId != shopId) throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // --- DOMAIN LOGIC ---
        appointment.Complete();

        // --- PERSISTENCE WITH CONCURRENCY CHECK ---
        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // This exception is thrown by EF Core if the RowVersion has changed.
            // We translate it into a more specific exception for the API layer to handle.
            throw new ConcurrencyException("ConcurrencyConflict");
        }
    }
}