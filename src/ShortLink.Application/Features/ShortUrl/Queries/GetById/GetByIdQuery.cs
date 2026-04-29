
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetById;

public class GetByIdQuery : IRequest<QueryResponse?>
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "id is required")]
    public Guid Id { get; set; }
    public GetByIdQuery(Guid id)
    {
        Id = id;
    }
}
