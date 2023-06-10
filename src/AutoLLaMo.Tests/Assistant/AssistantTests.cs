using AutoLLaMo.Actors.User;
using AutoLLaMo.Model.Messages.Chats;
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

    [Fact]
    public async Task AIAgentRequestsNameWhenStartingChat()
    {
        // Arrange
        await StartAsync();
        var userActorProps = ActorSystem.DI().PropsFor<IUserActor>();

        var userActor = ActorSystem.Root.SpawnNamed(userActorProps, nameof(IUserActor));

        // Act
        // var response = 

        // Assert
        // response.Lines.Should().HaveCount(5);
        // response.Lines.Should().Contain("Welcome to AutoLlamo!");
        // response.Lines.Should().Contain("Enter the name of your AI assistant, its role, and goals below.");
        // response.Lines.Should().Contain("Name: (for example, AutoLlamo Developer)");

        userActor.Should().NotBeNull();

        await FinishAsync();
    }
}
