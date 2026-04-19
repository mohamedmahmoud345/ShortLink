using Dapper;
using Microsoft.EntityFrameworkCore;
using ShortLink.Domain.Common.Analytics;
using ShortLink.Domain.Entities;
using ShortLink.Domain.Interfaces.Repositories;
using ShortLink.Infrastructure.Dapper;
using ShortLink.Infrastructure.Data;

namespace ShortLink.Infrastructure.Repositories;

public class ClickEventRepository : IClickEventRepository
{
    private readonly AppDbContext _context;
    private readonly DapperContext _dapperContext;
    public ClickEventRepository(AppDbContext context, DapperContext dapperContext)
    {
        _context = context;
        _dapperContext = dapperContext;
    }
    public async Task<ClickEvent> CreateAsync(ClickEvent clickEvent)
    {
        _context.ClickEvents.Add(clickEvent);
        await _context.SaveChangesAsync();
        return clickEvent;
    }

    public async Task<IEnumerable<ClickEvent>> GetByUrlIdAsync(Guid urlId, int page, int pageSize)
    {
        return await _context.ClickEvents
            .Where(x => x.ShortUrlId == urlId)
            .OrderByDescending(x => x.ClickedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetClickCountAsync(Guid urlId)
    {
        return await _context.ClickEvents.CountAsync(x => x.ShortUrlId == urlId);
    }

    public async Task<IEnumerable<CountryStats>> GetCountryStatsAsync(Guid urlId)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(Country, 'Unknown') AS Country,
            COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortUrlId = @urlId
            GROUP BY Country
            ORDER BY Count DESC          
        ";

        return await connection.QueryAsync<CountryStats>(query, new { urlId });
    }

    public async Task<IEnumerable<DailyClickStats>> GetDailyClicksAsync(Guid urlId, int days)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT CAST(ClickedAt AS DATE) AS Date, COUNT(*) AS COUNT
            FROM ClickEvents
            WHERE ShortUrlId = @urlId AND ClickedAt >= @StartDate
            GROUP BY CAST(ClickedAt As DATE)
            ORDER BY Date DESC
        ";

        return await connection.QueryAsync<DailyClickStats>(query, new { urlId, StartDate = DateTime.Now.AddDays(-days) });
    }

    public async Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(Guid urlId)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(DeviceType, 'Unknown') AS DeviceType, COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortUrlId = @urlId
            GROUP BY DeviceType
            ORDER BY Count DESC
        ";

        return await connection.QueryAsync<DeviceStats>(query, new { urlId });
    }

    public async Task<IEnumerable<ReferrerStats>> GetTopReferrersAsync(Guid urlId, int limit)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(Referrer, '(direct)') AS Referrer, COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortUrlId = @urlId
            GROUP BY Referrer
            ORDER BY Count DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY
        ";
        return await connection.QueryAsync<ReferrerStats>(query, new { urlId, Limit = limit });
    }
}
