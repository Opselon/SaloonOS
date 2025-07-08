using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SaloonOS.Api.Extensions;
using SaloonOS.Application; // <-- For AddApplicationServices()
using SaloonOS.Infrastructure; // <-- For AddInfrastructureServices()

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURE SERVICES ---

// Register services from other layers using our new DI modules.
// This is incredibly clean and maintainable.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);


// Register services specific to the API layer.
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. CONFIGURE THE HTTP REQUEST PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("fa-IR"), new CultureInfo("ru-RU") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseApiKeyAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();