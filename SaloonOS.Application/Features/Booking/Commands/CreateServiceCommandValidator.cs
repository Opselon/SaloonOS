using FluentValidation;

namespace SaloonOS.Application.Features.Booking.Commands;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("ServiceNameRequired");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("ServicePriceInvalid");
        RuleFor(x => x.DurationInMinutes).GreaterThan(0).WithMessage("ServiceDurationInvalid");
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}