using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Events;

namespace SaloonOS.Application.Features.Booking.Commands;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IPublisher _publisher;
    public DeleteServiceCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext, IPublisher publisher) // <-- Update constructor
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _publisher = publisher; 
    }

    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();
        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId);

        if (service is null || service.ShopId != tenantId)
        {
            throw new NotFoundException("Service", request.ServiceId);
        }

        _unitOfWork.Services.Remove(service);
        await _unitOfWork.CompleteAsync();

        // --- PUBLISH EVENT ---
        await _publisher.Publish(new ServiceDeletedEvent(service.Id, service.ShopId), cancellationToken);
    }
}