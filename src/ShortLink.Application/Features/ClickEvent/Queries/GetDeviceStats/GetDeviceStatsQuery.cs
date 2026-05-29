
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;

public class GetDeviceStatsQuery : IRequest<List<GetDeviceStatsResponse>>
{
    public GetDeviceStatsQuery(Guid userId, Guid urlId)
    {
        UserId = userId;
        UrlId = urlId;
    }

    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
}
