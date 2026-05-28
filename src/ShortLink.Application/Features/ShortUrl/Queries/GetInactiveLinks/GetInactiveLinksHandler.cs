
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetInactiveLinks;

public class GetInactiveLinksHandler : IRequestHandler<GetInactiveLinksQuery, List<QueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetInactiveLinksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<QueryResponse>> Handle(GetInactiveLinksQuery request, CancellationToken cancellationToken)
    {
        var links = await _unitOfWork.ShortUrls.GetInactiveLinksAsync(request.UserId);
        if (!links.Any())
            return [];

        var result = links.Select(x => new QueryResponse()
        {
            Id = x.Id,
            OriginalLink = x.OriginalLink,
            ShortCode = x.ShortCode,
            CreatedAt = x.CreatedAt,
            ExpiresAt = x.ExpiresAt,
            IsActive = x.IsActive,
            Clicks = x.Clicks
        }).ToList();

        return result;
    }
}
