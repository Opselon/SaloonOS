using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Infrastructure.Persistence;
using SaloonOS.Infrastructure.Persistence.DbContext;

namespace SaloonOS.Infrastructure;

/// <summary>
/// DI registration module for the Infrastructure layer.
/// It is responsible for registering services like the DbContext, repositories,
/// the Unit of Work, and any clients for external services (e.g., email, payments).
/// It can take the IConfiguration object to access connection strings and API keys.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure the DbContext
        var connectionString = configuration.GetConnectionString("SaloonOSDb");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'SaloonOSDb' not found.");
        }
        services.AddDbContext<SaloonOSDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register the Unit of Work with a Scoped lifetime, appropriate for web requests.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Note: Specific repositories are managed by the UnitOfWork and don't need individual registration.

        return services;
    }
}