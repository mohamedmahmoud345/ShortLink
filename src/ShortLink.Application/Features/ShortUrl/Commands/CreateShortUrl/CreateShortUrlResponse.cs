
namespace ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;

public class CreateShortUrlResponse
{
    public Guid Id { get; set; }
    public string OriginalLink { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int Clicks { get; set; }
}
