using Microsoft.EntityFrameworkCore;
using ShortLink.Domain.Entities;
using ShortLink.Domain.Interfaces.Repositories;
using ShortLink.Infrastructure.Data;

namespace ShortLink.Infrastructure.Repositories;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly AppDbContext _context;
    public ShortUrlRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ShortUrl> CreateAsync(ShortUrl shortUrl)
    {
        await _context.ShortUrls.AddAsync(shortUrl);
        await _context.SaveChangesAsync();
        return shortUrl;
    }
    public async Task<ShortUrl?> GetByIdAsync(Guid id)
    {
        return await _context.ShortUrls
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
    {
        return await _context.ShortUrls
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode);
    }
    public async Task<IEnumerable<ShortUrl>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.ShortUrls
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    public async Task<IEnumerable<ShortUrl>> GetAllAsync()
    {
        return await _context.ShortUrls
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    public async Task<ShortUrl> UpdateAsync(ShortUrl shortUrl)
    {
        _context.ShortUrls.Update(shortUrl);
        await _context.SaveChangesAsync();
        return shortUrl;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var shortUrl = await _context.ShortUrls.FindAsync(id);
        if (shortUrl == null) return false;

        _context.ShortUrls.Remove(shortUrl);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsAsync(string shortCode)
    {
        return await _context.ShortUrls.AnyAsync(x => x.ShortCode == shortCode);
    }
    public async Task<bool> IsOwnedByUserAsync(Guid linkId, Guid userId)
    {
        return await _context.ShortUrls.AnyAsync(x => x.Id == linkId && x.UserId == userId);
    }
}

