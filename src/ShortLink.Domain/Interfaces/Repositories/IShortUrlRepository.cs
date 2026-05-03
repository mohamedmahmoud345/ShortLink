
using ShortLink.Domain.Entities;

namespace ShortLink.Domain.Interfaces.Repositories;

public interface IShortUrlRepository
{
    Task<ShortUrl> CreateAsync(ShortUrl shortUrl);
    Task<ShortUrl?> GetByIdAsync(Guid id);
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
    Task<IEnumerable<ShortUrl>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<ShortUrl>> GetAllAsync(); // for admin
    Task<ShortUrl> UpdateAsync(ShortUrl shortUrl);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string shortCode);
    Task<ShortUrl?> GetByIdForUserAsync(Guid urlId, Guid userId);
}
