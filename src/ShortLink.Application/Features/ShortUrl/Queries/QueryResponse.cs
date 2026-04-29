
namespace ShortLink.Application.Features.ShortUrl.Queries.GetById;

public class QueryResponse
{
    public Guid Id { get; set; }
    public string OriginalLink { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int Clicks { get; set; }
}
