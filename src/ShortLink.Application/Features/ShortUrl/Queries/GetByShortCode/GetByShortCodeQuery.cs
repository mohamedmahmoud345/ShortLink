
using System.ComponentModel.DataAnnotations;
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetByShortCode;

public class GetByShortCodeQuery : IRequest<QueryResponse>
{
    public string ShortCode { get; set; }

    public GetByShortCodeQuery(string shortCode)
    {
        ShortCode = shortCode;
    }
}
