// Path: SaloonOS.Api/Program.cs
// =================================================================================================
// FILE: SaloonOS.Api/Program.cs
// PURPOSE: This is the composition root for the SaloonOS Web API.
// =================================================================================================

using System.Globalization;
using Hangfire;
using Hangfire.PostgreSql;
using SaloonOS.Api.Authorization;
using SaloonOS.Api.Extensions;
using SaloonOS.Application;
using SaloonOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICE CONFIGURATION (Dependency Injection) ---
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("SaloonOSDb"))));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
    options.ServerName = "SaloonOS.Background.Server";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SaloonOS API", Version = "v1" });
});

var app = builder.Build();

// --- HTTP REQUEST PIPELINE CONFIGURATION ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaloonOS API v1"));
}

app.UseHttpsRedirection();

var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("fa-IR"), new CultureInfo("ru-RU") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllAuthorizationFilter() }
});

app.UseApiKeyAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- RUN THE API ---
await app.RunAsync();