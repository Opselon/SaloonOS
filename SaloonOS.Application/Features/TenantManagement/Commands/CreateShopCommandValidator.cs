using FluentValidation;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

public class CreateShopCommandValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Shop name cannot be empty.")
            .MaximumLength(100).WithMessage("Shop name cannot exceed 100 characters.");

        RuleFor(x => x.ApiKey).NotEmpty().MinimumLength(32).WithMessage("API Key must be at least 32 characters for security.");

        RuleFor(x => x.DefaultLanguageCode).NotEmpty().Must(BeAValidLanguageCode).WithMessage("Please provide a valid language code (e.g., 'en-US', 'fa-IR').");

        // Add validation for the BusinessCategoryId.
        RuleFor(x => x.BusinessCategoryId)
            .NotEmpty().WithMessage("BusinessCategoryRequired") // Use localized message.
            .NotEqual(Guid.Empty).WithMessage("BusinessCategoryRequired");
    }

    private bool BeAValidLanguageCode(string code)
    {
        return !string.IsNullOrWhiteSpace(code) && code.Contains('-');
    }
}