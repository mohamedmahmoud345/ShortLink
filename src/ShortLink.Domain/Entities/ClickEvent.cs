
namespace ShortLink.Domain.Entities;

public class ClickEvent
{
    public Guid Id { get; set; }
    public Guid ShortLinkId { get; set; }
    public DateTime ClickedAt { get; set; }
    public string Referrer { get; set; }
    public string County { get; set; }
    public string DeviceType { get; set; }
    public string IpAddress { get; set; }
    
    public ShortLink ShortLink { get; set; }
}
