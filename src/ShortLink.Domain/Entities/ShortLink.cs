
namespace ShortLink.Domain.Entities;

public class ShortLink
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAd { get; set; }
    public bool IsActive { get; set; }
    public int Clicks { get; set; }

    public User User { get; set; }
    public ICollection<ClickEvent> ClickEvents { get; set; }
}
