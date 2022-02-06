using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WordLimiter.Core.Behaviours;
using WordLimiter.Core.Services;
using System.Reflection;

namespace WordLimiter.Core;

public static class _DependencyRegister
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(_DependencyRegister)) ?? throw new Exception("This should literally not happen....");

        services.AddSingleton<IWordLimiterService, WordLimiterService>();
        services.AddSingleton<IWordScoreService, WordScoreService>();
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(assembly);

        // Set up mediator pipeline behaviours (order matters):
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}