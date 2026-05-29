
using ShortLink.Domain.Enums;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;

public class GetByUrlIdResponse
{
    public Guid Id { get; set; }
    public Guid ShortUrlId { get; set; }
    public DateTime ClickedAt { get; set; }
    public string Referrer { get; set; }
    public string Country { get; set; }
    public DeviceType DeviceType { get; set; }
    public string IpAddress { get; set; }
}
