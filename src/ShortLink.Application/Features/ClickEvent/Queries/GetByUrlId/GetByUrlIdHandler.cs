
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;

public class GetByUrlIdHandler : IRequestHandler<GetByUrlIdQuery, List<GetByUrlIdResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetByUrlIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<GetByUrlIdResponse>> Handle(GetByUrlIdQuery request, CancellationToken cancellationToken)
    {
        var isExists = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (isExists == null)
            throw new NotFoundException($"The short URL with ID '{request.UrlId}' was not found.");

        var clickEvents = await _unitOfWork.ClickEvents.GetByUrlIdAsync(request.UrlId, request.Page, request.PageSize);

        if (!clickEvents.Any())
            return [];

        var result = clickEvents.Select(x => new GetByUrlIdResponse()
        {
            Id = x.Id,
            ShortUrlId = x.ShortUrlId,
            ClickedAt = x.ClickedAt,
            Referrer = x.Referrer,
            IpAddress = x.IpAddress,
            Country = x.Country,
            DeviceType = x.DeviceType
        }).ToList();

        return result;
    }
}
