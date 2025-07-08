using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Api.Resources; // For SharedResources
using SaloonOS.Infrastructure.Persistence.DbContext;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURE SERVICES ---

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- DATABASE CONFIGURATION (PostgreSQL) ---
var connectionString = builder.Configuration.GetConnectionString("SaloonOSDb");
builder.Services.AddDbContext<SaloonOSDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- REPOSITORY & UNIT OF WORK REGISTRATION (Add this later when they are built) ---
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


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

app.UseAuthorization();

app.MapControllers();

app.Run();