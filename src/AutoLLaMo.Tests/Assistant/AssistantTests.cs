using AutoLLaMo.Actors.User;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Plugins;
using FluentAssertions;
using Proto;
using Proto.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AutoLLaMo.Tests.Assistant;

public class AssistantTests : TestBase
{
    public AssistantTests(ITestOutputHelper output) : base(output)
    {
    }

    // see https://gist.github.com/xpando/2571365 for retries
    [Fact]
    public async Task AssistantRequestsUserDesireWhenStartingChat()
    {
        // Arrange
        await StartAsync();
        var userActorProps = ActorSystem.DI().PropsFor<IUserActor>();

        // Act
        var userActor = ActorSystem.Root.SpawnNamed(userActorProps, nameof(IUserActor));

        // Assert
        await Task.Delay(1000);
        var messages = await ActorSystem.Root.RequestAsync<List<Message>>(userActor, new GetMessages());

        messages.Should().ContainSingle(message => message is GetUserDesire);

        await FinishAsync();
    }
}
