
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDailyClicks;

public class GetDailyClicksQuery : IRequest<List<GetDailyClicksResponse>>
{
    public GetDailyClicksQuery(Guid userId, Guid urlId, DateTime? date)
    {
        UserId = userId;
        UrlId = urlId;
        Date = date ?? DateTime.Today;
    }

    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
    public DateTime Date { get; set; }

}
