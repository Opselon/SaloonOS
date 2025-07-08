namespace SaloonOS.Application.Common.Contracts;

/// <summary>
/// Defines a contract for a distributed caching service. This abstraction allows us
/// to switch the caching provider (e.g., Redis, Memcached) without changing application code.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
}