using Microsoft.Extensions.DependencyInjection;
using ShortLink.Application.Features.Admin.Interfaces;
using ShortLink.Application.Services;
using ShortLink.Application.Services.Auth;
using ShortLink.Domain.Interfaces.Repositories;
using ShortLink.Domain.Interfaces.UnitOfWork;
using ShortLink.Infrastructure.Dapper;
using ShortLink.Infrastructure.Repositories;
using ShortLink.Infrastructure.Repositories.Admin;
using ShortLink.Infrastructure.Services;
using ShortLink.Infrastructure.Services.Auth;
using ShortLink.Infrastructure.Workers;

namespace ShortLink.Infrastructure.Dependencies;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IClickEventRepository, ClickEventRepository>();
        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
        services.AddScoped<IUnitOfWork, UnitofWork.UnitOfWork>();
        services.AddSingleton<DapperContext>();
        services.AddScoped<IGetAllUsersReadRepository, GetAllUsersReadRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddHostedService<LinkCleanupService>();

        return services;
    }
}
