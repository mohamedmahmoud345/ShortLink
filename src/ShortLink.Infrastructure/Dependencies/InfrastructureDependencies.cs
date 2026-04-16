using Microsoft.Extensions.DependencyInjection;
using ShortLink.Domain.Interfaces.Repositories;
using ShortLink.Domain.Interfaces.UnitOfWork;
using ShortLink.Infrastructure.Dapper;
using ShortLink.Infrastructure.Repositories;

namespace ShortLink.Infrastructure.Dependencies;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IClickEventRepository, ClickEventRepository>();
        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
        services.AddScoped<IUnitOfWork, UnitofWork.UnitOfWork>();
        services.AddSingleton<DapperContext>();
        
        return services;
    }
}
