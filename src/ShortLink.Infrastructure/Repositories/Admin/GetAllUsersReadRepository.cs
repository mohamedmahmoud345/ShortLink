using Dapper;
using ShortLink.Application.DTOs.Admin;
using ShortLink.Application.Features.Admin.Interfaces;
using ShortLink.Infrastructure.Dapper;


namespace ShortLink.Infrastructure.Repositories.Admin;

public class GetAllUsersReadRepository : IGetAllUsersReadRepository
{
    private readonly DapperContext _dapperContext;
    public GetAllUsersReadRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }
    public async Task<IEnumerable<UserInfoDto>> GetAllUsersAsync()
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT
            u.Id AS UserId,
            COALESCE(u.UserName, '') AS UserName,
            COALESCE(u.Email, '') AS Email,
            COUNT(su.Id) AS NumberOfUrls,
            COALESCE(r.Name, 'User') AS Role
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur
            on ur.UserId = u.Id
            LEFT JOIN AspNetRoles r 
            on r.Id = ur.RoleId
            LEFT JOIN ShortUrls su
            ON su.UserId = u.Id
            GROUP BY u.Id, u.UserName, u.Email, r.Name
            ORDER BY u.UserName
        ";

        return await connection.QueryAsync<UserInfoDto>(query, new { });
    }
}
