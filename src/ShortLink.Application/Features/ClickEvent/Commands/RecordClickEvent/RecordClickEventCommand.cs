
using MediatR;

namespace ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;

public class RecordClickEventCommand : IRequest<bool>
{
    public RecordClickEventCommand(string shortCode, string referrer, string ipAddress, string userAgent)
    {
        ShortCode = shortCode;
        Referrer = referrer;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public string ShortCode { get; set; }
    public string Referrer { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
