using FluentValidation;

namespace SaloonOS.Application.Features.TenantManagement.Commands;

/// <summary>
/// Provides validation rules for the CreateShopCommand using FluentValidation.
/// This ensures that incoming data is well-formed before it hits the core business logic.
/// The rules are declarative and highly readable.
/// </summary>
public class CreateShopCommandValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Shop name cannot be empty.")
            .MaximumLength(100).WithMessage("Shop name cannot exceed 100 characters.");

        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("API Key cannot be empty.")
            .MinimumLength(32).WithMessage("API Key must be at least 32 characters for security.");

        RuleFor(x => x.DefaultLanguageCode)
            .NotEmpty()
            .Must(BeAValidLanguageCode).WithMessage("Please provide a valid language code (e.g., 'en-US', 'fa-IR').");
    }

    private bool BeAValidLanguageCode(string code)
    {
        // A simple check. In a real system, you might validate against a list of supported cultures.
        return !string.IsNullOrWhiteSpace(code) && code.Contains('-');
    }
}