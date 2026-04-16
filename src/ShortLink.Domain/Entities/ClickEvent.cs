
using ShortLink.Domain.Enums;

namespace ShortLink.Domain.Entities;

public class ClickEvent
{
    public Guid Id { get; set; }
    public Guid ShortLinkId { get; set; }
    public DateTime ClickedAt { get; set; }
    public string Referrer { get; set; }
    public string Country { get; set; }
    public DeviceType DeviceType { get; set; }
    public string IpAddress { get; set; }

    public ShortUrl ShortUrl { get; set; }
}
