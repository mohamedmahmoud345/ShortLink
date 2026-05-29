
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetTopReferrers;

public class GetTopReferrersHandler : IRequestHandler<GetTopReferrersQuery, List<GetTopReferrersResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTopReferrersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<GetTopReferrersResponse>> Handle(GetTopReferrersQuery request, CancellationToken cancellationToken)
    {
        var isExists = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (isExists == null)
            throw new NotFoundException($"The short URL with ID '{request.UrlId}' was not found.");

        if (request.Limit <= 0) request.Limit = 5;

        var topReferrers = await _unitOfWork.ClickEvents.GetTopReferrersAsync(request.UrlId, request.Limit);

        var result = topReferrers.Select(x => new GetTopReferrersResponse()
        {
            Referrer = x.Referrer,
            Count = x.Count
        }).ToList();

        return result;
    }
}
