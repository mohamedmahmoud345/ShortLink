
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;

public class GetByUrlIdQuery :IRequest<List<GetByUrlIdResponse>>
{
    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public GetByUrlIdQuery(Guid userId, Guid urlId, int page , int pageSize)
    {
        UserId = userId;
        UrlId = urlId;
        Page = page;
        PageSize = pageSize;
    }
}
