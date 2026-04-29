
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;

public class CreateShortUrlCommand : IRequest<CreateShortUrlResponse?>
{
    public CreateShortUrlCommand(Guid userId, string originalLink)
    {
        UserId = userId;
        OriginalLink = originalLink;
    }

    public Guid UserId { get; set; }
    public string OriginalLink { get; set; }

}
