
using MediatR;
using Microsoft.Extensions.Logging;
using ShortLink.Application.Common;
using ShortLink.Application.Services;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;

public class UpdateHandler : IRequestHandler<UpdateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly ILogger<UpdateHandler> _logger;
    public UpdateHandler(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<UpdateHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cacheService;
        _logger = logger;
    }
    public async Task<bool> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);

        if (url is null)
            throw new NotFoundException($"The link with ID '{request.UrlId}' was not found.");


        url.OriginalLink = request.Url;

        await _unitOfWork.ShortUrls.UpdateAsync(url);

        var key = $"link:{url.ShortCode}";
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to invalidate cache key {CacheKey}",
                key);
        }

        return true;
    }
}
