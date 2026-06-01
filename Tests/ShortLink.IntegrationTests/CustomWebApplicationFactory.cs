
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortLink.Infrastructure.Data;
using ShortLink.Infrastructure.Data.Identity;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ShortLink.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:conStr"] = _dbContainer.GetConnectionString(),
                ["ConnectionStrings:RedisConnection"] = _redisContainer.GetConnectionString(),
                ["Jwt:SecretKey"] = "test-secret-key-please-chaNge3423#",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["SeedAdmin:Email"] = "admin@shortlink.com",
                ["SeedAdmin:Password"] = "SecureAdminPass123!",
                ["INTERNAL_SECURE_TOKEN"] = "test-internal-token"
            };

            config.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(
               d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );

            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });
    }
    public async Task InitializeAsync()
    {
        // Start both containers in parallel for maximum speed
        await Task.WhenAll(
            _dbContainer.StartAsync(),
            _redisContainer.StartAsync()
        );

        // 2. Build a temporary service provider just to apply migrations BEFORE the main application host boots up
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(_dbContainer.GetConnectionString());
        });

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // This forces the creation of AspNetRoles and all tables before Program.cs runs!
        await db.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Task.WhenAll(
            _dbContainer.StopAsync(),
            _redisContainer.StopAsync()
        );
    }
}