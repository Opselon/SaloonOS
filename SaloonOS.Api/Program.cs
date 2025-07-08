using System.Globalization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Api.Extensions; // <-- ADD THIS
using SaloonOS.Api.Resources;
using SaloonOS.Application.Common.Behaviours;
using SaloonOS.Application.Common.Contracts; // <-- ADD THIS
using SaloonOS.Api.Services; // <-- ADD THIS
using SaloonOS.Infrastructure.Persistence;
using SaloonOS.Infrastructure.Persistence.DbContext;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURE SERVICES ---
builder.Services.AddHttpContextAccessor(); // <-- ADD THIS. Crucial for TenantContext.

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// --- APPLICATION SERVICES REGISTRATION ---
var applicationAssembly = AppDomain.CurrentDomain.Load("SaloonOS.Application");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// --- INFRASTRUCTURE & CUSTOM SERVICES REGISTRATION ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITenantContext, TenantContext>(); // <-- ADD THIS

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SaloonOSDb");
builder.Services.AddDbContext<SaloonOSDbContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

// --- 2. CONFIGURE THE HTTP REQUEST PIPELINE ---
var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("fa-IR"), new CultureInfo("ru-RU") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); // Ensure routing is configured.

// Place our custom middleware in the correct order: AFTER routing, BEFORE authorization/endpoints.
app.UseApiKeyAuthentication(); // <-- ADD THIS

app.UseAuthorization();
app.MapControllers();

app.Run();