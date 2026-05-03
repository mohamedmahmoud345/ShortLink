
using MediatR;

namespace ShortLink.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<GetAllUsersReponse>>
{
}
