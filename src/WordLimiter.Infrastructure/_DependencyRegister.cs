using Microsoft.Extensions.DependencyInjection;
using WordLimiter.Core.Dependencies;
using WordLimiter.Infrastructure.Data;

namespace WordLimiter.Infrastructure;

public static class _DependencyRegister
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWordRepository, WordRepository>();

        return services;
    }
}