using System.Text;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Actors.Assistant.Prompts.AssistantRole;

public static class CommandExtensions
{
    public static string ToCompactDescription(this IPluginSignature pluginSignature)
    {
        var description = new StringBuilder();

        var commandSchema = pluginSignature.CommandType.ToJsonSchema();
        var commandSchemaJson = commandSchema.ToJson();

        description.AppendLine($"\"{commandSchema.Title}\":");
        description.AppendLine("Command schema:");
        description.AppendLine($"{commandSchemaJson}");

        var outputSchema = pluginSignature.OutputType.ToJsonSchema();

        description.AppendLine("Output:");
        description.AppendLine($"{outputSchema.Description}");

        return description.ToString();
    }
}
