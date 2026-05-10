
using MediatR;
using ShortLink.Application.Services;
using ShortLink.Domain.Enums;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;

public class RecordClickEventHandler : IRequestHandler<RecordClickEventCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoIpService _geoIp;
    public RecordClickEventHandler(IUnitOfWork unitOfWork, IGeoIpService geoIpService)
    {
        _unitOfWork = unitOfWork;
        _geoIp = geoIpService;
    }
    public async Task<bool> Handle(RecordClickEventCommand request, CancellationToken cancellationToken)
    {
        var shortUrl = await _unitOfWork.ShortUrls.GetByShortCodeAsync(request.ShortCode);
        if (shortUrl is null)
            return false;

        var deviceType = ParseDeviceType(request.UserAgent);

        var country = await _geoIp.GetCountryByIpAsync(request.IpAddress);

        var ClickEvent = new Domain.Entities.ClickEvent()
        {
            Id = Guid.NewGuid(),
            ShortUrlId = shortUrl.Id,
            ClickedAt = DateTime.UtcNow,
            Referrer = string.IsNullOrWhiteSpace(request.Referrer) ? "Direct" : request.Referrer,
            IpAddress = request.IpAddress,
            Country = country,
            DeviceType = deviceType
        };

        await _unitOfWork.ClickEvents.CreateAsync(ClickEvent);

        return true;
    }


    private DeviceType ParseDeviceType(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return DeviceType.Desktop;

        var ua = userAgent.ToLower();

        if (ua.Contains("mobi") || ua.Contains("iphone") || ua.Contains("android"))
        {
            return DeviceType.Mobile;
        }
        if (ua.Contains("ipad") || ua.Contains("tablet"))
        {
            return DeviceType.Tablet;
        }

        return DeviceType.Desktop;
    }
}
