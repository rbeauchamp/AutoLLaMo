using AutoLLaMo.Actors.User;
using AutoLLaMo.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoLLaMo.Tests.Assistant;

public class TestStartup : Startup
{
    public override void ConfigureServices(IServiceCollection services, string envFilePath)
    {
        base.ConfigureServices(
            services,
            envFilePath);

        services.RemoveAll(typeof(IUserActor));
        services.AddTransient<IUserActor, TestUserActor>();
    }
}
