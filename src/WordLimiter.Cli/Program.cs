using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WordLimiter.Core;
using WordLimiter.Infrastructure;

namespace WordLimiter.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        var host = BuildHost();
        host.Run();
        return 0;
    }

    private static IHost BuildHost()
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
                loggerBuilder.AddConsole();
            })
            .ConfigureServices((context, serviceBuilder) =>
            {
                serviceBuilder.AddInfrastructure();
                serviceBuilder.AddCore();

                serviceBuilder.AddHostedService<WordLimiterHostedService>();
            });

        return hostBuilder.Build();
    }
}