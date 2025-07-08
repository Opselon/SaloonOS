using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using System.Linq;

namespace SaloonOS.Application.Features.Booking.Queries;

public class GetCustomerAppointmentsQueryHandler : IRequestHandler<GetCustomerAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetCustomerAppointmentsQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetCustomerAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var shopId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        var appointments = await _unitOfWork.Appointments.GetUpcomingAppointmentsForCustomerAsync(shopId, request.CustomerId);

        // Map the domain entities to DTOs
        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            StartTime = a.StartTime,
            Status = a.Status.ToString(),
            CustomerId = a.CustomerId,
            ServiceId = a.ServiceId,
            StaffId = a.StaffMemberId
        });
    }
}