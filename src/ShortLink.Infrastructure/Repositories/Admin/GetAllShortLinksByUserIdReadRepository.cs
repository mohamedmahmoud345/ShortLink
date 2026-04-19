using Dapper;
using ShortLink.Application.Features.Admin.Interfaces;
using ShortLink.Domain.Entities;
using ShortLink.Infrastructure.Dapper;

namespace ShortLink.Infrastructure.Repositories.Admin;

public class GetAllShortLinksByUserIdReadRepository : IGetAllShortUrlsByUserIdReadRepository
{
    private readonly DapperContext _dapperContext;
    public GetAllShortLinksByUserIdReadRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }
    public async Task<IEnumerable<ShortUrl>> GetByUserIdAsync(Guid userId)
    {
        using var connection = _dapperContext.CreateConnection();
        var query = @"
            SELECT * 
            FROM ShortUrls 
            WHERE UserId = @userId
            ORDER BY CreatedAt DESC
        ";

        return await connection.QueryAsync<ShortUrl>(query, new { userId });
    }
}
