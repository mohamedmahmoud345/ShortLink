
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDailyClicks;

public class GetDailyClicksHanlder : IRequestHandler<GetDailyClicksQuery, List<GetDailyClicksResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetDailyClicksHanlder(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<GetDailyClicksResponse>> Handle(GetDailyClicksQuery request, CancellationToken cancellationToken)
    {
        var isExists = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (isExists == null)
            throw new NotFoundException($"The short URL with ID '{request.UrlId}' was not found.");

        var dailyClicks = await _unitOfWork.ClickEvents.GetDailyClicksAsync(request.UrlId, request.Date);
        var result = dailyClicks.Select(x => new GetDailyClicksResponse()
        {
            Count = x.Count,
            Date = x.Date
        }).ToList();

        return result;
    }
}
