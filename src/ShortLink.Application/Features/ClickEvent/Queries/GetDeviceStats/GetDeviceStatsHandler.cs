
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;

public class GetDeviceStatsHandler : IRequestHandler<GetDeviceStatsQuery, List<GetDeviceStatsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetDeviceStatsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<GetDeviceStatsResponse>> Handle(GetDeviceStatsQuery request, CancellationToken cancellationToken)
    {
        var isExists = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (isExists == null)
            throw new NotFoundException($"The short URL with ID '{request.UrlId}' was not found.");

        var deviceStats = await _unitOfWork.ClickEvents.GetDeviceStatsAsync(request.UrlId);

        var result = deviceStats.Select(x => new GetDeviceStatsResponse()
        {
            DeviceType = x.DeviceType,
            Count = x.Count
        }).ToList();

        return result;
    }
}
