
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetTopReferrers;

public class GetTopReferrersQuery : IRequest<List<GetTopReferrersResponse>>
{
    public GetTopReferrersQuery(Guid userId, Guid urlId, int limit)
    {
        UserId = userId;
        UrlId = urlId;
        Limit = limit;
    }

    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
    public int Limit { get; set; }
}
