
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

    public ICollection<ClickEvent> ClickEvents { get; set; } = new List<ClickEvent>();


    public ShortUrl(Guid userId, string originalLink, DateTime? expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OriginalLink = originalLink;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
        Clicks = 0;
    }

    public void AddShortCode(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            throw new ArgumentNullException(nameof(shortCode));
        if (ShortCode != null)
            throw new InvalidOperationException("can not add short code twice");

        ShortCode = shortCode;
    }
}
