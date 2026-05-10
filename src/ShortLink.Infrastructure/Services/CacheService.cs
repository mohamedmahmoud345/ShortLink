
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ShortLink.Application.Services;

namespace ShortLink.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var cached = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cached)) return default;

        return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
    }
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
        else
            options.SetAbsoluteExpiration(TimeSpan.FromHours(6));

        var json = JsonSerializer.Serialize(value, _jsonOptions);

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

}
