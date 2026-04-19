
namespace ShortLink.Domain.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OriginalLink { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int Clicks { get; set; }

    public ICollection<ClickEvent> ClickEvents { get; set; }
}
