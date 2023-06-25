using System.Text.Json;
using System.Text.Json.Serialization;
using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
using AutoLLaMo.Plugins;
using Proto;
using Proto.Extensions;

namespace AutoLLaMo.Actors.Plugin;

public class PluginActor : IActor
{
    private readonly IEnumerable<Plugins.Plugin> _plugins;

    public PluginActor(IEnumerable<Plugins.Plugin> plugins)
    {
        _plugins = plugins;
    }

    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Started:
            case Restarting:
                break;
            case NextCommandApproved nextCommandApproved:
                var response = await ExecuteNextCommandAsync(
                    nextCommandApproved.NextCommand,
                    context.CancellationToken);
                context.Send<AssistantActor>(
                    new NextCommandExecuted(
                        response,
                        nextCommandApproved));
                break;
            default:
                throw new InvalidStateException(
                    $"{nameof(PluginActor)} cannot handle {context.Message.GetMessageTypeName()}");
        }
    }

    private Task<Response> ExecuteNextCommandAsync(
        NextCommand nextCommand,
        CancellationToken cancellationToken)
    {
        // Get plugin
        var plugin =
            _plugins.SingleOrDefault(p => p.Signature.CommandType.Name == nextCommand.Name)
            ?? throw new InvalidStateException($"Plugin {nextCommand.Name} not found");

        // Instantiate and populate command
        var command = InstantiateAndPopulateCommand(
            plugin,
            nextCommand);

        // Invoke plugin
        return plugin.ExecuteAsync(
            command,
            cancellationToken);
    }

    private static Command InstantiateAndPopulateCommand(
        Plugins.Plugin plugin,
        NextCommand nextCommand)
    {
        return JsonSerializer.Deserialize(
                   nextCommand.Json,
                   plugin.Signature.CommandType, new JsonSerializerOptions
                   {
                       WriteIndented = true,
                       DefaultIgnoreCondition =
                           JsonIgnoreCondition.WhenWritingNull,
                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                   }) as Command
               ?? throw new InvalidStateException(
                   $"Plugin {plugin.Signature.CommandType} does not have a valid command type");
    }
}
