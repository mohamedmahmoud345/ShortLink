
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.RefreshLink;

public class RefreshLinkHandler : IRequestHandler<RefreshLinkCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RefreshLinkHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RefreshLinkCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (url is null)
            throw new NotFoundException($"The link with ID '{request.UrlId}' was not found.");

        var now = DateTime.UtcNow;
        if (url.ExpiresAt > now)
            return false;
        else
        {
            var newExpiry = now.AddDays(30);
            url.IsActive = true;
            url.ExpiresAt = newExpiry;

            await _unitOfWork.ShortUrls.UpdateAsync(url);
            return true;
        }
    }
}
