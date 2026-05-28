
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetById;

public class GetByIdQuery : IRequest<QueryResponse>
{
    public Guid Id { get; set; }
    public GetByIdQuery(Guid id)
    {
        Id = id;
    }
}
