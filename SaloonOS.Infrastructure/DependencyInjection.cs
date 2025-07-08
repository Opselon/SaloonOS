// --- REQUIRED USING DIRECTIVES ---
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options; // <-- ADD THIS USING for manual binding
using SaloonOS.Application.Common.Configuration;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Infrastructure.Caching;
using SaloonOS.Infrastructure.Persistence;
using SaloonOS.Infrastructure.Persistence.DbContext;
using SaloonOS.Infrastructure.Services;
using StackExchange.Redis;
using Hangfire;
using Hangfire.PostgreSql;

namespace SaloonOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // --- DATABASE CONFIGURATION (PostgreSQL - The "Write" Side) ---
        var connectionString = configuration.GetConnectionString("SaloonOSDb");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Fatal Error: Database connection string 'SaloonOSDb' not found in configuration.");
        }
        services.AddDbContext<SaloonOSDbContext>(options =>
            options.UseNpgsql(connectionString));

        // --- CACHING CONFIGURATION (Redis - The "Read" Side) ---
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new InvalidOperationException("Fatal Error: Redis connection string 'Redis' not found in configuration.");
        }
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, RedisCacheService>();

        // --- PAYMENT SERVICE CONFIGURATION (Stripe) ---
        // Register our abstracted payment service.
        services.AddScoped<IPaymentService, StripePaymentService>();

        // ============================ CORRECTED SECTION ============================
        // We will now manually bind the configuration section and register it as a singleton IOptions<T>.
        // This bypasses the problematic extension method and achieves the same result.

        // 1. Create a new instance of our settings class.
        var stripeSettings = new StripeSettings();

        // 2. Explicitly get the "Stripe" section from configuration and bind its values to our instance.
        configuration.GetSection("Stripe").Bind(stripeSettings);

        // 3. Register the populated settings object as a singleton wrapped in IOptions<T>.
        //    This makes it available for injection as IOptions<StripeSettings> everywhere else.
        services.AddSingleton(Options.Create(stripeSettings));
        // ===========================================================================

        // --- UNIT OF WORK & REPOSITORIES ---
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAppointmentReminderJob, SaloonOS.Application.Features.Notifications.AppointmentReminderJob>();
        services.AddScoped<INotificationService, TelegramNotificationService>();

        services.AddHangfire(config => config
      .UseSimpleAssemblyNameTypeSerializer()
      .UseRecommendedSerializerSettings()
      .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(configuration.GetConnectionString("SaloonOSDb"))));

        services.AddHangfireServer();

        // Register our custom background job so Hangfire's DI can resolve it.
        services.AddScoped<IAppointmentReminderJob, SaloonOS.Application.Features.Notifications.AppointmentReminderJob>();


        return services;
    }
}