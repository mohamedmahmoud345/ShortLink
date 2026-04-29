
using System.ComponentModel.DataAnnotations;
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetAllByUserId;

public class GetAllQuery : IRequest<List<QueryResponse>>
{

    [Required(AllowEmptyStrings = false, ErrorMessage = "id is required")]
    public Guid UserId { get; set; }
    public GetAllQuery(Guid userId)
    {
        UserId = userId;
    }
}
