using System.Diagnostics;
using Proto;
using Proto.DependencyInjection;

namespace AutoLLaMo.Common;

public static class ActorContextExtensions
{
    public static async Task RequestAsync<TActor>(
        this IContext context,
        object message)
        where TActor : IActor
    {
        var actor = context.GetActor<TActor>();

        var timeoutSeconds = Debugger.IsAttached ? 600 : 300;

        var cancellationTokenSource = new CancellationTokenSource(timeoutSeconds * 1000);

        try
        {
            await context.RequestAsync<bool>(
                actor,
                message,
                cancellationTokenSource.Token);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }
    }

    public static void Forward<TActor>(this IContext context)
        where TActor : IActor
    {
        var actor = context.GetActor<TActor>();

        context.Forward(
            actor);
    }

    public static void Send<TActor>(this IContext actorContext, object message)
        where TActor : IActor
    {
        // Signal that the response is complete
        actorContext.Respond(message: true);

        var actor = actorContext.GetActor<TActor>();

        actorContext.Send(
            actor,
            message);
    }

    private static PID GetActor<TActor>(this IContext context)
        where TActor : IActor
    {
        var actorName = typeof(TActor).Name;

        // check sender
        if (context.Sender?.Id.EndsWith(actorName) == true)
        {
            return context.Sender;
        }

        // then check parent
        if (context.Parent?.Id.EndsWith(actorName) == true)
        {
            return context.Parent;
        }

        // then check for a child
        if (context.Children.Any(child => child.Id.EndsWith(actorName)))
        {
            return context.Children.Single(child => child.Id.EndsWith(actorName));
        }

        var props = context.System.DI().PropsFor<TActor>();

        return context.SpawnNamed(
            props,
            actorName);
    }
}
