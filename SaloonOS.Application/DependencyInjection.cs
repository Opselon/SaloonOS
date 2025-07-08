using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SaloonOS.Application.Common.Behaviours;

namespace SaloonOS.Application;

/// <summary>
/// A modern approach to DI registration. This static class provides an extension method
/// on IServiceCollection to encapsulate all service registrations for the Application layer.
/// This keeps Program.cs clean and respects the boundaries of Clean Architecture.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        // Register MediatR and all its handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        // Register the custom ValidationBehavior to the MediatR pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register all FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }
}