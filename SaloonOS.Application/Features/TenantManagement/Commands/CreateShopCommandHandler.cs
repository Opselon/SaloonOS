using MediatR;
using Microsoft.Extensions.Logging; // For logging
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Application.Exceptions; // For custom exceptions
using SaloonOS.Application.Features.TenantManagement.Commands;
using SaloonOS.Domain.TenantManagement.Entities;
using SaloonOS.Domain.TenantManagement.Events; // For ShopCreatedEvent

public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly ILogger<CreateShopCommandHandler> _logger;

    public CreateShopCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher, ILogger<CreateShopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Business Category Existence (Crucial step missing before)
        // We need to ensure the provided BusinessCategoryId is valid.
        var businessCategory = await _unitOfWork.GetRepository<BusinessCategory>().GetByIdAsync(request.BusinessCategoryId);
        if (businessCategory is null)
        {
            // Use a specific exception for invalid category ID.
            throw new ArgumentException($"Business category with ID '{request.BusinessCategoryId}' not found.", nameof(request.BusinessCategoryId));
        }

        // 2. Domain Logic: Create the shop using the factory method, now including the category.
        var shop = Shop.Create(
            request.Name,
            request.ApiKey,
            request.DefaultLanguageCode,
            request.BusinessCategoryId // Pass the validated category ID.
        );

        // 3. Persistence: Add the new entity.
        await _unitOfWork.Shops.AddAsync(shop);
        await _unitOfWork.CompleteAsync();

        // 4. Publish Event: Notify other parts of the system.
        await _publisher.Publish(new ShopCreatedEvent(shop), cancellationToken);

        _logger.LogInformation("Shop '{ShopName}' created successfully with ID: {ShopId}", shop.Name, shop.Id);
        return shop.Id;
    }
}