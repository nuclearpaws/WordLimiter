using Microsoft.Extensions.DependencyInjection;

namespace WordLimiter.Core;

public static class _DependencyRegister
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IWordLimiter, WordLimiter>();

        return services;
    }
}