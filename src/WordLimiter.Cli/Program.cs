using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WordLimiter.Cli.ArgumentOptions;
using WordLimiter.Core;
using WordLimiter.Infrastructure;

namespace WordLimiter.Cli;

public static class Program
{
    public static async Task<int> Main(
        string[] args)
    {
        var globalLogger = LoggerFactory
            .Create(builder => {
                builder.AddConsole();
            })
            .CreateLogger(typeof(Program));

        try
        {
            await Parser
                .Default
                .ParseArguments<DefaultArgs>(args)
                .WithParsedAsync<DefaultArgs>(async parsedArgs =>
                {
                    var host = BuildHost(parsedArgs);
                    await host.RunAsync();
                });

            return 0;
        }
        catch(Exception ex)
        {
            globalLogger.LogCritical(ex, "The program has run into an unexpected exception.");
            return 1;
        }
    }

    private static IHost BuildHost(DefaultArgs args)
    {
        var hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(configBuilder =>
            {
            })
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
            })
            .ConfigureLogging((context, loggerBuilder) =>
            {
                loggerBuilder.AddFilter((provider, category, logLevel) => {
                    return category.Contains(nameof(WordLimiter));
                });
                loggerBuilder.AddConsole();
            })
            .ConfigureServices((context, serviceBuilder) =>
            {
                serviceBuilder.AddInfrastructure();
                serviceBuilder.AddCore(coreOptions =>
                {
                    coreOptions.Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
                    coreOptions.CommonLetters = new char[] { 'e', 't', 'a', 'i', 'o', 'n', 's', 'h', 'r' };
                    coreOptions.CommonFirstLetters = new char[] { 't', 'a', 'o', 'd', 'w' };
                    coreOptions.CommonLastLetters = new char[] { 'e', 's', 'd', 't' };
                    coreOptions.VowelScore = 3;
                    coreOptions.CommonLetterScore = 5;
                    coreOptions.CommonFirstLetterScore = 7;
                    coreOptions.CommonLastLetterScore = 7;
                    coreOptions.DuplicateLetterScore = -10;
                });

                serviceBuilder.AddSingleton(args);
                serviceBuilder.AddHostedService<WordLimiterHostedService>();
            });

        return hostBuilder.Build();
    }
}