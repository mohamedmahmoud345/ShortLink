
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetCountryStats;

public class GetCountryStatsQuery : IRequest<List<GetCountryStatsResponse>>
{
    public GetCountryStatsQuery(Guid userId, Guid urlId)
    {
        UserId = userId;
        UrlId = urlId;
    }

    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
}
