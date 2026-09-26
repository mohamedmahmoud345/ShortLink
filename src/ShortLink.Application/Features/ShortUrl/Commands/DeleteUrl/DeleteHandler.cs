
using MediatR;
using Microsoft.Extensions.Logging;
using ShortLink.Application.Common;
using ShortLink.Application.Services;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.DeleteUrl;

public class DeleteHandler : IRequestHandler<DeleteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly ILogger<DeleteHandler> _logger;
    public DeleteHandler(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<DeleteHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cacheService;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (url is null)
            throw new NotFoundException($"The Link with ID '{request.UrlId}' was not found.");

        url.IsActive = false;

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
