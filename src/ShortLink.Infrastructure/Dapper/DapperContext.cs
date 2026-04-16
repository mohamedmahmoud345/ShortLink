
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ShortLink.Infrastructure.Dapper;

public class DapperContext
{
    private readonly string _connectionString;
    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("conStr") ?? throw new InvalidOperationException("connection string not found");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
