
namespace ShortLink.Application.Services;
/// <summary>Go owns link:* via cache-aside. C# only invalidates via RemoveAsync. Get/Set retained.</summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}
