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

    // public static void PopulateCommandPropertiesFromArgs(
    //     Command command,
    //     NextCommand nextCommand)
    // {
    //     // Get the type of the command
    //     var commandType = command.GetType();
    //
    //
    //
    //     // // Iterate over the Args in the NextCommandResponse
    //     // foreach (var arg in nextCommand.Args)
    //     // {
    //     //     // Try to get the property on the command
    //     //     var property = commandType.GetProperty(arg.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    //     //
    //     //     // If the property exists and can be written to
    //     //     if (property?.CanWrite == true)
    //     //     {
    //     //         // Get the type of the property
    //     //         var propertyType = property.PropertyType;
    //     //
    //     //         // Try to convert the Arg's value to the property's type and set the property value
    //     //         try
    //     //         {
    //     //             object? convertedValue = null;
    //     //
    //     //             if (propertyType == typeof(List<string>) && arg.Value != null)
    //     //             {
    //     //                 convertedValue = ConvertStringToList(arg.Value);
    //     //             }
    //     //             else
    //     //             {
    //     //                 convertedValue = Convert.ChangeType(
    //     //                     arg.Value,
    //     //                     propertyType);
    //     //             }
    //     //
    //     //             property.SetValue(
    //     //                 command,
    //     //                 convertedValue);
    //     //         }
    //     //         catch (Exception ex)
    //     //         {
    //     //             // Handle or log the exception as necessary
    //     //             Console.WriteLine($"Failed to set property {arg.Name}: {ex.Message}");
    //     //         }
    //     //     }
    //     // }
    // }

    // private static List<string> ConvertStringToList(string str)
    // {
    //     if (string.IsNullOrEmpty(str))
    //     {
    //         return new List<string>();
    //     }
    //
    //     return str.Split(separator: ',').Select(s => s.Trim()).ToList();
    // }
}