
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetCountryStats;

public class GetCountryStatsHandler : IRequestHandler<GetCountryStatsQuery, List<GetCountryStatsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCountryStatsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<GetCountryStatsResponse>> Handle(GetCountryStatsQuery request, CancellationToken cancellationToken)
    {
        var isExists = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (isExists == null)
            throw new NotFoundException($"The short URL with ID '{request.UrlId}' was not found.");

        var countryStats = await _unitOfWork.ClickEvents.GetCountryStatsAsync(request.UrlId);

        var result = countryStats.Select(x => new GetCountryStatsResponse()
        {
            Country = x.Country,
            Count = x.Count
        }).ToList();

        return result;
    }
}
