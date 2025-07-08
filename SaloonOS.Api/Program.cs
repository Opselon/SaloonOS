using System.Globalization;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Api.Resources;
using SaloonOS.Application.Common.Behaviours;
using SaloonOS.Application.Common.Contracts;
using SaloonOS.Infrastructure.Persistence;
using SaloonOS.Infrastructure.Persistence.DbContext;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURE SERVICES ---

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// --- APPLICATION SERVICES REGISTRATION ---
// Get the assembly where the application layer handlers and validators are located.
var applicationAssembly = AppDomain.CurrentDomain.Load("SaloonOS.Application");

// Register MediatR and all its handlers from the Application assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

// Register the custom ValidationBehavior to the MediatR pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Register all FluentValidation validators from the Application assembly
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// --- INFRASTRUCTURE SERVICES REGISTRATION ---
// Register the Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Note: Repositories are managed by the UnitOfWork, so we only need to register the UoW.

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- DATABASE CONFIGURATION (PostgreSQL) ---
var connectionString = builder.Configuration.GetConnectionString("SaloonOSDb");
builder.Services.AddDbContext<SaloonOSDbContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

// --- 2. CONFIGURE THE HTTP REQUEST PIPELINE ---

// Configure Localization Middleware
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("fa-IR"),
    new CultureInfo("ru-RU")
};

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

// Add global exception handling middleware here later
app.UseAuthorization();

app.MapControllers();

app.Run();