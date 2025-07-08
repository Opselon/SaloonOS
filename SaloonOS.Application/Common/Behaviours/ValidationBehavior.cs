using FluentValidation;
using MediatR;

namespace SaloonOS.Application.Common.Behaviours;

/// <summary>
/// A MediatR pipeline behavior that automatically validates incoming requests.
/// This is a powerful cross-cutting concern that keeps validation logic out of the command handlers.
/// If any validators are registered for a given IRequest, this behavior will execute them.
/// If validation fails, it throws a ValidationException, preventing the request from reaching the handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            // If there are no validators for this request type, just continue the pipeline.
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Run all validators and collect failures.
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // If there are any validation failures, throw an exception.
            // This can be caught by a global exception handling middleware in the API layer.
            throw new ValidationException(failures);
        }

        return await next();
    }
}