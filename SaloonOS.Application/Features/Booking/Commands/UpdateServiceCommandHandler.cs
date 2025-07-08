using MediatR;
using Microsoft.EntityFrameworkCore; // <-- ADD THIS USING
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions;
using SaloonOS.Domain.Booking.Events; // <-- ADD THIS USING

namespace SaloonOS.Application.Features.Booking.Commands;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IPublisher _publisher; 
    public UpdateServiceCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext, IPublisher publisher) // <-- Update constructor
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _publisher = publisher; // <-- Assign publisher
    }
    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException();

        // The FindBy method now exists and can be used.
        var service = await _unitOfWork.Services.FindBy(s => s.Id == request.ServiceId && s.ShopId == tenantId)
                                                .Include(s => s.Translations)
                                                .FirstOrDefaultAsync(cancellationToken);

        if (service is null)
        {
            // The NotFoundException is now defined and can be used.
            throw new NotFoundException("Service", request.ServiceId);
        }

        var shop = await _unitOfWork.Shops.GetByIdAsync(tenantId) ?? throw new InvalidOperationException("Shop not found for tenant.");

        // Use the powerful domain method to perform the update.
        service.AddOrUpdateTranslation(shop.DefaultLanguageCode, request.Name, request.Description);

        // In the future, you would add service.UpdatePrice(request.Price); etc.

        await _unitOfWork.CompleteAsync();
        await _publisher.Publish(new ServiceUpdatedEvent(service!), cancellationToken);
    }
}