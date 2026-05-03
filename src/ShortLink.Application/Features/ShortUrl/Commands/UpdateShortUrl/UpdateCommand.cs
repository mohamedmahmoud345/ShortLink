
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;

public class UpdateCommand : IRequest<bool>
{
    public Guid UrlId { get; set; }
    public Guid UserId { get; set; }
    public string Url { get; set; }
    public UpdateCommand(Guid urlid, Guid userId, string url)
    {
        UrlId = urlid;
        UserId = userId;
        Url = url;
    }
}
