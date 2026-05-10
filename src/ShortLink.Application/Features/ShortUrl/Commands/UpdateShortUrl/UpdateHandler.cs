
using MediatR;
using ShortLink.Application.Services;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;

public class UpdateHandler : IRequestHandler<UpdateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    public UpdateHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cache = cacheService;
    }
    public async Task<bool> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);

        if (url is null)
            return false;

        url.OriginalLink = request.Url;

        await _unitOfWork.ShortUrls.UpdateAsync(url);

        var key = $"link:{url.ShortCode}";
        await _cache.RemoveAsync(key);

        return true;
    }
}
