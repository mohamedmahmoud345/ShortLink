
using MediatR;
using Microsoft.Extensions.Logging;
using ShortLink.Application.Services;
using ShortLink.Domain.Enums;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;

public class RecordClickEventHandler : IRequestHandler<RecordClickEventCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoIpService _geoIp;
    private readonly ILogger<RecordClickEventHandler> _logger;
    public RecordClickEventHandler(IUnitOfWork unitOfWork, IGeoIpService geoIpService, ILogger<RecordClickEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _geoIp = geoIpService;
        _logger = logger;
    }
    public async Task<bool> Handle(RecordClickEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Recording click shortCode={shortCode} ip={ip}", request.ShortCode, request.IpAddress);

        var shortUrl = await _unitOfWork.ShortUrls.GetByShortCodeAsync(request.ShortCode);
        if (shortUrl is null)
        {
            _logger.LogWarning("ClickEvent target not found shortCode={shortCode}", request.ShortCode);
            return false;
        }

        try
        {
            var deviceType = ParseDeviceType(request.UserAgent);
            var country = await _geoIp.GetCountryByIpAsync(request.IpAddress);

            var clickEvent = new Domain.Entities.ClickEvent
            {
                Id = Guid.NewGuid(),
                ShortUrlId = shortUrl.Id,
                ClickedAt = DateTime.UtcNow,
                Referrer = string.IsNullOrWhiteSpace(request.Referrer) ? "Direct" : request.Referrer,
                IpAddress = request.IpAddress,
                Country = country,
                DeviceType = deviceType
            };

            await _unitOfWork.ClickEvents.CreateAsync(clickEvent);

            // persist counter explicitly
            shortUrl.Clicks++;
            await _unitOfWork.ShortUrls.UpdateAsync(shortUrl);

            _logger.LogDebug("Recorded click shortCode={shortCode} newClicks={clicks}", request.ShortCode, shortUrl.Clicks);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed recording click for shortCode={shortCode}", request.ShortCode);
            return false;
        }
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
