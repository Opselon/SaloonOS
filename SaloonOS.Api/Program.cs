// =================================================================================================
// FILE: SaloonOS.Api/Program.cs
//
// PURPOSE: This is the composition root and entry point for the SaloonOS API.
//          It is responsible for building the application host, configuring all services (DI),
//          and defining the HTTP request processing pipeline (middleware).
//
// ARCHITECTURAL PRINCIPLE: This file should remain as clean as possible. Complex service
//                          registrations are delegated to extension methods in their
//                          respective project layers (e.g., AddApplicationServices).
// =================================================================================================

// --- SECTION 1: USING DIRECTIVES ---
// Import all necessary namespaces for configuration.
using System.Globalization;
using Hangfire;
using Hangfire.PostgreSql;
using SaloonOS.Api.Authorization; // For Hangfire Dashboard Authorization
using SaloonOS.Api.Extensions;
using SaloonOS.Application;
using SaloonOS.Infrastructure;

// --- SECTION 2: APPLICATION BUILDER SETUP ---
// The WebApplication.CreateBuilder initializes a new instance of the WebApplicationBuilder
// with preconfigured defaults, loading appsettings.json, environment variables, etc.
var builder = WebApplication.CreateBuilder(args);

// --- SECTION 3: SERVICE CONFIGURATION (Dependency Injection) ---
// This is where we register all the services our application will use.

// Register services from other layers using our custom DI modules.
// This keeps our Program.cs clean and respects architectural boundaries.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

// Register Hangfire for background job processing.
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180) // Recommended for compatibility
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("SaloonOSDb"))));

// Register the Hangfire server process to run in the background.
builder.Services.AddHangfireServer(options =>
{
    // Configure server options here if needed.
    options.WorkerCount = Environment.ProcessorCount * 2; // A sensible default
    options.ServerName = "SaloonOS.Background.Server";
});

// Register services specific to the API (Presentation) layer.
builder.Services.AddHttpContextAccessor(); // Essential for services that need access to the current HTTP context.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Future step: Add Swagger configuration for bearer token auth, etc.
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SaloonOS API", Version = "v1" });
});

// --- SECTION 4: APPLICATION BUILD ---
// The Build() method constructs the WebApplication instance which will handle requests.
var app = builder.Build();

// --- SECTION 5: HTTP REQUEST PIPELINE CONFIGURATION ---
// This section defines the order of middleware that will process every incoming request.
// Order is critical.

// In a development environment, enable developer-friendly tools.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // More detailed error pages for developers.
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaloonOS API v1"));
}

// Enforce HTTPS for security.
app.UseHttpsRedirection();

// Configure request localization to automatically set the culture based on request headers.
var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("fa-IR"), new CultureInfo("ru-RU") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Enable routing to map requests to endpoints.
app.UseRouting();

// Add the Hangfire Dashboard.
// IMPORTANT: This dashboard MUST be secured in a production environment.
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // The AllowAllAuthorizationFilter type can now be found.
    Authorization = new[] { new AllowAllAuthorizationFilter() }
});

// Add our custom middleware for API key authentication.
app.UseApiKeyAuthentication();

// Add standard ASP.NET Core authorization middleware.
app.UseAuthorization();

// Map incoming requests to their corresponding controller actions.
app.MapControllers();

// --- SECTION 6: RUN THE APPLICATION ---
// This final command starts the web server and begins listening for HTTP requests.
// We wrap it in a try-catch block for graceful shutdown and error logging.
try
{
    app.Logger.LogInformation("Starting SaloonOS API Host...");
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly.");
}
finally
{
    // In case of using Serilog or other loggers that need flushing.
    // Log.CloseAndFlush();
}