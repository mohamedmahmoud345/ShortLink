
using MediatR;
using ShortLink.Application.Features.Admin.Interfaces;

namespace ShortLink.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<GetAllUsersReponse>>
{
    private readonly IGetAllUsersReadRepository _repo;
    public GetAllUsersHandler(IGetAllUsersReadRepository repository)
    {
        _repo = repository;
    }
    public async Task<IEnumerable<GetAllUsersReponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repo.GetAllUsersAsync();
        if (!users.Any())
            return [];

        var res = users.Select(x => new GetAllUsersReponse
        {
            UserId = x.UserId,
            UserName = x.UserName,
            Email = x.Email,
            NumberOfUrls = x.NumberOfUrls,
            Role = x.Role
        });

        return res;
    }
}

