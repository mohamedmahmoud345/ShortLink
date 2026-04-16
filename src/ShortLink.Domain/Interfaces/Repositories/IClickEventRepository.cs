using ShortLink.Domain.Common.Analytics;
using ShortLink.Domain.Entities;


namespace ShortLink.Domain.Interfaces.Repositories;

public interface IClickEventRepository
{
    Task<ClickEvent> CreateAsync(ClickEvent clickEvent);
    Task<IEnumerable<ClickEvent>> GetByLinkIdAsync(Guid linkId, int page, int pageSize);
    Task<int> GetClickCountAsync(Guid linkId);
    Task<IEnumerable<DailyClickStats>> GetDailyClicksAsync(Guid linkId, int days);
    Task<IEnumerable<ReferrerStats>> GetTopReferrersAsync(Guid linkId, int limit);
    Task<IEnumerable<CountryStats>> GetCountryStatsAsync(Guid linkId);
    Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(Guid linkId);
}
