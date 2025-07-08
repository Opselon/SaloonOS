using MediatR;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.DTOs;
using SaloonOS.Domain.Booking.Entities;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateServiceCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        // 1. Authorization & Context: Ensure a valid tenant is making the request.
        var tenantId = _tenantContext.TenantId ?? throw new UnauthorizedAccessException("Cannot create a service without an authenticated tenant.");
        var shop = await _unitOfWork.Shops.GetByIdAsync(tenantId) ?? throw new InvalidOperationException($"Shop not found for Tenant ID: {tenantId}");

        // 2. Domain Logic: Use the domain's factory method to create the Service aggregate.
        // This enforces all business rules and invariants at the point of creation.
        var service = Service.Create(
            shopId: shop.Id,
            price: request.Price,
            currency: request.Currency,
            durationInMinutes: request.DurationInMinutes,
            initialLanguageCode: shop.DefaultLanguageCode, // Use the shop's default language for the first translation.
            initialName: request.Name,
            initialDescription: request.Description
        );

        // 3. Persistence: Add the newly created aggregate to the repository.
        // EF Core's change tracker will automatically detect the related ServiceTranslation and add it as well.
        await _unitOfWork.Services.AddAsync(service);

        // 4. Transaction: Commit all changes to the database as a single, atomic operation.
        await _unitOfWork.CompleteAsync();

        // 5. Mapping & Response: Map the persisted entity to a DTO for the API response.
        // It's crucial to return the data from the object that was just saved, including its new ID.
        var translation = service.Translations.First(); // We know there is at least one.
        return new ServiceDto
        {
            Id = service.Id,
            Name = translation.Name,
            Description = translation.Description,
            Price = service.Price,
            Currency = service.Currency,
            DurationInMinutes = service.DurationInMinutes
        };
    }
}