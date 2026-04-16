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

    public async Task<IEnumerable<ClickEvent>> GetByLinkIdAsync(Guid linkId, int page, int pageSize)
    {
        return await _context.ClickEvents
            .Where(x => x.ShortLinkId == linkId)
            .OrderByDescending(x => x.ClickedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetClickCountAsync(Guid linkId)
    {
        return await _context.ClickEvents.CountAsync(x => x.ShortLinkId == linkId);
    }

    public async Task<IEnumerable<CountryStats>> GetCountryStatsAsync(Guid linkId)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(Country, 'Unknown') AS Country,
            COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortLinkId = @linkId
            GROUP BY Country
            ORDER BY Count DESC          
        ";

        return await connection.QueryAsync<CountryStats>(query, new { linkId });
    }

    public async Task<IEnumerable<DailyClickStats>> GetDailyClicksAsync(Guid linkId, int days)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT CAST(ClickedAt AS DATE) AS Date, COUNT(*) AS COUNT
            FROM ClickEvents
            WHERE ShortLinkId = @linkId AND ClickedAt >= @StartDate
            GROUP BY CAST(ClickedAt As DATE)
            ORDER BY Date DESC
        ";

        return await connection.QueryAsync<DailyClickStats>(query, new { linkId, StartDate = DateTime.Now.AddDays(-days) });
    }

    public async Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(Guid linkId)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(DeviceType, 'Unknown') AS DeviceType, COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortLinkId = @linkId
            GROUP BY DeviceType
            ORDER BY Count DESC
        ";

        return await connection.QueryAsync<DeviceStats>(query, new { linkId });
    }

    public async Task<IEnumerable<ReferrerStats>> GetTopReferrersAsync(Guid linkId, int limit)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT ISNULL(Referrer, '(direct)') AS Referrer, COUNT(*) AS Count
            FROM ClickEvents
            WHERE ShortLinkId = @linkId
            GROUP BY Referrer
            ORDER BY Count DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY
        ";
        return await connection.QueryAsync<ReferrerStats>(query, new { LinkId = linkId, Limit = limit });
    }
}
