using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WordLimiter.Core.Behaviours;
using WordLimiter.Core.Services;
using System.Reflection;

namespace WordLimiter.Core;

public static class _DependencyRegister
{
    public static IServiceCollection AddCore(this IServiceCollection services, Action<CoreOptions>? optionsDelegate = default)
    {
        var coreOptions = new CoreOptions();
        optionsDelegate?.Invoke(coreOptions);

        var assembly = Assembly.GetAssembly(typeof(_DependencyRegister)) ?? throw new Exception("This should literally not happen....");

        services.AddSingleton<IWordLimiterService, WordLimiterService>();
        services.AddSingleton<IWordScoreService>(sp => new WordScoreService(
            coreOptions.Vowels,
            coreOptions.CommonLetters,
            coreOptions.CommonFirstLetters,
            coreOptions.CommonLastLetters,
            coreOptions.VowelScore,
            coreOptions.CommonLetterScore,
            coreOptions.CommonFirstLetterScore,
            coreOptions.CommonLastLetterScore,
            coreOptions.DuplicateLetterScore));
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(assembly);

        // Set up mediator pipeline behaviours (order matters):
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}

public sealed class CoreOptions
{
    public IEnumerable<char> Vowels { get; set; }
    public IEnumerable<char> CommonLetters { get; set; }
    public IEnumerable<char> CommonFirstLetters { get; set; }
    public IEnumerable<char> CommonLastLetters { get; set; }
    public int VowelScore { get; set; }
    public int CommonLetterScore { get; set; }
    public int CommonFirstLetterScore { get; set; }
    public int CommonLastLetterScore { get; set; }
    public int DuplicateLetterScore { get; set; }

    public CoreOptions()
    {
        Vowels = new List<char>();
        CommonLetters = new List<char>();
        CommonFirstLetters = new List<char>();
        CommonLastLetters = new List<char>();
    }
}