
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Commands.RefreshLink;

public class RefreshLinkCommand : IRequest<bool>
{
    public RefreshLinkCommand(Guid userId, Guid urlId)
    {
        UserId = userId;
        UrlId = urlId;
    }

    public Guid UserId { get; set; }
    public Guid UrlId { get; set; }
}
