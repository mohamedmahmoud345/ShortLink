
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetInactiveLinks;

public class GetInactiveLinksQuery : IRequest<List<QueryResponse>>
{
    public Guid UserId { get; set; }

    public GetInactiveLinksQuery(Guid userId)
    {
        UserId = userId;
    }
}   
