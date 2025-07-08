// Path: Core/Services/ApiClient.cs
using SaloonOS.Application.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SaloonOS.TelegramBot.Core.Services;

/// <summary>
/// A typed client responsible for all HTTP communication with the backend SaloonOS API.
/// It encapsulates the logic for serialization, deserialization, and error handling.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SaloonOsApi");
        _logger = logger;
    }

    public async Task<BotConfigurationDto?> GetBotConfigurationAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/configuration/bot");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BotConfigurationDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get bot configuration from API.");
            return null;
        }
    }

    public async Task<CustomerDto?> GetOrCreateCustomer(object command)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/customers/get-or-create", jsonContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get or create customer.");
            return null;
        }
    }

    // Add other methods for each API endpoint as they are needed...
    // public async Task<IEnumerable<ServiceDto>> ListServicesAsync() { ... }
}