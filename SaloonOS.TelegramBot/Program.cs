// Path: Program.cs
using SaloonOS.TelegramBot.Core;
using SaloonOS.TelegramBot.Core.Configuration;
using SaloonOS.TelegramBot.Core.Services;
using SaloonOS.TelegramBot.Features.Onboarding;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // --- Bind Strongly-Typed Configuration ---
        var botSettings = configuration.GetSection("BotSettings").Get<BotSettings>()!;
        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>()!;

        // --- DI Registration ---
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botSettings.Token));

        services.AddHttpClient("SaloonOsApi", client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("X-Api-Key", apiSettings.ApiKey);
        });

        // Core Services (Singletons or Scoped)
        services.AddSingleton<StateService>();
        services.AddScoped<ApiClient>();
        services.AddScoped<UpdateDispatcher>();

        // Feature Handlers (Scoped so they can have scoped dependencies like ApiClient)
        services.AddScoped<StartCommandHandler>();
        // services.AddScoped<MainMenuHandler>();
        // services.AddScoped<BookingConversationHandler>();

        // Main Background Service
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();