
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ShortLink.Application.Dependencies;

public static class ApplicationDependencies
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


        return services;
    }
}
