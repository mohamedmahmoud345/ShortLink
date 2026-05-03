
using System.ComponentModel.DataAnnotations;
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetByShortCode;

public class GetByShortCodeQuery : IRequest<QueryResponse?>
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Short Code required")]
    public string ShortCode { get; set; }

    public GetByShortCodeQuery(string shortCode)
    {
        ShortCode = shortCode;
    }
}
