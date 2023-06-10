using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Actors.Configurator;
using AutoLLaMo.Actors.Plugin;
using AutoLLaMo.Actors.User;
using AutoLLaMo.Core.Plugins.CloneRepository;
using AutoLLaMo.Core.Plugins.GenerateNewCommand;
using AutoLLaMo.Core.Plugins.WriteToFiles;
using AutoLLaMo.Model;
using AutoLLaMo.Plugins;
using AutoLLaMo.Services.OpenAI;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.DependencyInjection;
using LoadOptions = DotNetEnv.LoadOptions;

namespace AutoLLaMo.ConsoleApp;

public class Startup
{
    public virtual void ConfigureServices(IServiceCollection services, string envFilePath)
    {
        var config = new ConfigurationBuilder().AddDotNetEnv(
            envFilePath,
            LoadOptions.TraversePath()).AddEnvironmentVariables().Build();

        var settings = config.Get<Settings>()
                       ?? throw new InvalidStateException("Settings not found");

        // Services
        services.AddSingleton(settings);
        services.AddOpenAi(openAiSettings =>
        {
            openAiSettings.ApiKey = settings.OpenAIApiKey;
        });
        services.AddSingleton<IOpenAIApi, OpenAIApi>();
        services.AddSingleton(
            serviceProvider => new ActorSystem().WithServiceProvider(serviceProvider));

        // Actors
        services.AddTransient<IUserActor, UserActor>();
        services.AddTransient<AssistantActor>();
        services.AddTransient<ConfiguratorActor>();
        services.AddTransient<PluginActor>();

        // Plugins
        services.AddTransient<Plugin, GenerateNewCommandPlugin>();
        services.AddTransient<Plugin, WriteToFilesPlugin>();
        services.AddTransient<Plugin, CloneRepositoryPlugin>();
    }
}
