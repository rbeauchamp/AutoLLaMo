using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Xunit.Abstractions;

namespace AutoLLaMo.Tests.Assistant;

public abstract class TestBase
{
    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected ActorSystem ActorSystem { get; private set; } = null!;

    protected ITestOutputHelper Output { get; }

    protected async Task StartAsync()
    {
        ActorSystem = await ConfigureActorSystemAsync();
    }

    protected async Task FinishAsync()
    {
        try
        {
            await ActorSystem.DisposeAsync();
        }
        catch
        {
            // Don't fail when finishing a test
        }
    }

    private static async Task<ActorSystem> ConfigureActorSystemAsync()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(
        services =>
                {
                    new TestStartup().ConfigureServices(
                        services,
                        ".env");
                })
            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error))
            .Build();

        // This enables you to initialize services via IHostApplicationLifetime.ApplicationStarted if needed
        await host.StartAsync();

        return host.Services.GetRequiredService<ActorSystem>();
    }
}
