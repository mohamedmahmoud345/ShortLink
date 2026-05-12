using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShortLink.Infrastructure.Data;

namespace ShortLink.Infrastructure.Workers;

public class LinkCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private readonly ILogger<LinkCleanupService> _logger;
    public LinkCleanupService(IServiceProvider serviceProvider, ILogger<LinkCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var now = DateTime.UtcNow;

                    var query = "UPDATE ShortUrls SET IsActive = 0 WHERE IsActive = 1 AND ExpiresAt IS NOT NULL AND ExpiresAt <= {0}";
                    var rows = await db.Database.ExecuteSqlRawAsync(query, new object[] { now },
                    cancellationToken: stoppingToken);
                    _logger.LogInformation("LinkCleanupService: deactivated {rows} expired links", rows);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LinkCleanupService error");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}