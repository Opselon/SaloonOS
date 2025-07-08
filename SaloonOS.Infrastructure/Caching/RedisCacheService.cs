using System.Text.Json;
using StackExchange.Redis;
using SaloonOS.Application.Common.Contracts;

namespace SaloonOS.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _redisDb;

    public RedisCacheService(IConnectionMultiplexer redisConnection)
    {
        _redisDb = redisConnection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var redisValue = await _redisDb.StringGetAsync(key);
        return redisValue.HasValue ? JsonSerializer.Deserialize<T>(redisValue!) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var jsonValue = JsonSerializer.Serialize(value);
        await _redisDb.StringSetAsync(key, jsonValue, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDb.KeyDeleteAsync(key);
    }
}