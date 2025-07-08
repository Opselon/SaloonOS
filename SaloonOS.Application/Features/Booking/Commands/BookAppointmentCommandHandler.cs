using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Commands;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public BookAppointmentCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<AppointmentDto> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException(nameof(Service), request.ServiceId);

        // Security check
        if (service.ShopId != shopId)
        {
            throw new NotFoundException(nameof(Service), request.ServiceId);
        }

        bool isSlotAvailable = await _unitOfWork.Appointments.IsSlotAvailable(
            shopId, request.StaffId, request.StartTime, service.DurationInMinutes);

        if (!isSlotAvailable)
        {
            throw new InvalidOperationException("TimeSlotNotAvailable");
        }

        var appointment = Appointment.Create(
            shopId,
            request.CustomerId,
            request.ServiceId,
            request.StaffId,
            request.StartTime,
            service.DurationInMinutes,
            service.Price,
            service.Currency);

        await _unitOfWork.Appointments.AddAsync(appointment);
        await _unitOfWork.CompleteAsync();

        // TODO: Publish AppointmentBookedEvent

        return new AppointmentDto
        {
            Id = appointment.Id,
            StartTime = appointment.StartTime,
            Status = appointment.Status.ToString(),
            CustomerId = appointment.CustomerId,
            ServiceId = appointment.ServiceId,
            StaffId = appointment.StaffMemberId
        };
    }
}