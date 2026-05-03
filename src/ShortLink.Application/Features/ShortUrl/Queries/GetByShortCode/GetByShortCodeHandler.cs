
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetByShortCode;

public class GetByShortCodeHandler : IRequestHandler<GetByShortCodeQuery, QueryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetByShortCodeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<QueryResponse?> Handle(GetByShortCodeQuery request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByShortCodeAsync(request.ShortCode);
        if (url is null)
            return null;


        return new QueryResponse()
        {
            Id = url.Id,
            OriginalLink = url.OriginalLink,
            ShortCode = url.ShortCode,
            CreatedAt = url.CreatedAt,
            ExpiresAt = url.ExpiresAt,
            IsActive = url.IsActive,
            Clicks = url.Clicks
        };
    }
}
