using AutoLLaMo.Actors.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Spectre;
using Log = Serilog.Log;

namespace AutoLLaMo.ConsoleApp;

public static class Program
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Spectre(restrictedToMinimumLevel: LogEventLevel.Error)
            .CreateLogger();

        // Configure ProtoActor to use Serilog
        using var loggerFactory = LoggerFactory.Create(l => l.AddSerilog().SetMinimumLevel(LogLevel.Error));
        Proto.Log.SetLoggerFactory(
            loggerFactory);

        var host = ConfigureActorSystem();

        // This enables you to initialize services via IHostApplicationLifetime.ApplicationStarted if needed
        await host.StartAsync();

        var actorSystem = host.Services.GetRequiredService<ActorSystem>();

        var userActorProps = actorSystem.DI().PropsFor<UserActor>();

        actorSystem.Root.SpawnNamed(userActorProps, nameof(UserActor));

        await host.WaitForShutdownAsync(actorSystem.Shutdown);

        await Log.CloseAndFlushAsync();
    }

    private static IHost ConfigureActorSystem()
    {
        return Host.CreateDefaultBuilder().ConfigureServices(
            services =>
            {
                Startup.ConfigureServices(
                    services,
                    ".env");
            }).ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error)).Build();
    }
}
