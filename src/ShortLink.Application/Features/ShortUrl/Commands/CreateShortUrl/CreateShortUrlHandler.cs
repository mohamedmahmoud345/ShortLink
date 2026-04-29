
using MediatR;
using ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl.GenerateCode;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;

public class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateShortUrlHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateShortUrlResponse?> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var shortUrlObj = new Domain.Entities.ShortUrl(request.UserId, request.OriginalLink, null);

        var shortCode = Base62.Encode(shortUrlObj.Id);

        var isExists = await _unitOfWork.ShortUrls.ExistsAsync(shortCode);
        if (isExists)
            return null;

        shortUrlObj.AddShortCode(shortCode);
        await _unitOfWork.ShortUrls.CreateAsync(shortUrlObj);

        return new CreateShortUrlResponse()
        {
            Id = shortUrlObj.Id,
            OriginalLink = shortUrlObj.OriginalLink,
            ShortCode = shortUrlObj.ShortCode,
            CreatedAt = shortUrlObj.CreatedAt,
            ExpiresAt = shortUrlObj.ExpiresAt,
            IsActive = shortUrlObj.IsActive,
            Clicks = shortUrlObj.Clicks
        };
    }
}
