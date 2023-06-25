using AutoLLaMo.Actors.User;
using Proto.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AutoLLaMo.Tests.Assistant;

public class AssistantTests : TestBase
{
    public AssistantTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task AssistantRequestsUserDesireWhenStartingChat()
    {
        // Arrange
        await StartAsync();
        var userActorProps = ActorSystem.DI().PropsFor<IUserActor>();

        // Act
        var userActor = ActorSystem.Root.SpawnNamed(userActorProps, nameof(IUserActor));

        // Assert
        //userActor.

        await FinishAsync();
    }
}
