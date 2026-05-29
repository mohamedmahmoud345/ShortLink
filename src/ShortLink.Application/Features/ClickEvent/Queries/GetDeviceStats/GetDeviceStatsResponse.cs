
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;

public class GetDeviceStatsResponse 
{
    public string DeviceType { get; set; } 
    public int Count { get; set; }
}
