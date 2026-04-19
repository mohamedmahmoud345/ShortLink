using ShortLink.Domain.Common.Analytics;
using ShortLink.Domain.Entities;


namespace ShortLink.Domain.Interfaces.Repositories;

public interface IClickEventRepository
{
    Task<ClickEvent> CreateAsync(ClickEvent clickEvent);
    Task<IEnumerable<ClickEvent>> GetByUrlIdAsync(Guid urlId, int page, int pageSize);
    Task<int> GetClickCountAsync(Guid urlId);
    Task<IEnumerable<DailyClickStats>> GetDailyClicksAsync(Guid urlId, int days);
    Task<IEnumerable<ReferrerStats>> GetTopReferrersAsync(Guid urlId, int limit);
    Task<IEnumerable<CountryStats>> GetCountryStatsAsync(Guid urlId);
    Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(Guid linurlIdkId);
}
