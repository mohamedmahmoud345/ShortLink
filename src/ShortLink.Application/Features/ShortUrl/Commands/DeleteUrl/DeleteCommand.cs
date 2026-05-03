
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Commands.DeleteUrl;

public class DeleteCommand : IRequest<bool>
{
    public Guid UrlId { get; set; }
    public Guid UserId { get; set; }
    public DeleteCommand(Guid urlid, Guid userId)
    {
        UrlId = urlid;
        UserId = userId;
    }
}
