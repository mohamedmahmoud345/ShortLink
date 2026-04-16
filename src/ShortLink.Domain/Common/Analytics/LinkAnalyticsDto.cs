namespace ShortLink.Domain.Common.Analytics;

public class LinkAnalyticsDto
{
    public int TotalClicks { get; set; }
    public List<DailyClickStats> DailyClicks { get; set; } = new();
    public List<ReferrerStats> TopReferrers { get; set; } = new();
    public List<CountryStats> Countries { get; set; } = new();
    public List<DeviceStats> Devices { get; set; } = new();
}