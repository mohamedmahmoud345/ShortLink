
using System.ComponentModel.DataAnnotations;
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetAllAdmin;

public class GetAllAdminQuery : IRequest<List<QueryResponse>>
{
}
