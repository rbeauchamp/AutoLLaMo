using System.Text;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Actors.Assistant.Prompts.AssistantRole;

public static class AssistantRolePromptFormatter
{
    public static string FormatContent(this AssistantRolePromptContent content)
    {
        var sb = new StringBuilder();

        sb.AppendLine(content.Role);
        sb.AppendLine(content.ResponseDirective);

        sb.AppendLine().AppendLine("Goals:").AppendLine(FormatAsNumberedList(content.Goals));

        sb.AppendLine().AppendLine("Constraints:")
            .AppendLine(FormatAsNumberedList(content.Constraints));

        sb.AppendLine().AppendLine("Important:")
            .AppendLine(FormatAsNumberedList(content.Important));

        sb.AppendLine().AppendLine("Useful Context:").AppendLine(content.UsefulContext);

        sb.AppendLine().AppendLine("Available Commands:").AppendLine(
            FormatAsNumberedList(
                content.AvailableCommands.ToCompactTextualDescriptions(),
                appendPeriodIfMissing: false));

        sb.AppendLine().AppendLine("Performance Evaluation:").AppendLine(
            FormatAsNumberedList(content.PerformanceEvaluationCriteria));

        sb.AppendLine().AppendLine(content.ResponseFormat);

        return sb.ToString().TrimEnd();
    }

    public static string FormatAsNumberedList(
        List<string> items,
        bool appendPeriodIfMissing = true)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (appendPeriodIfMissing && !item.EndsWith('.'))
            {
                item += ".";
            }

            builder.AppendLine($"{i + 1}. {item}");
        }

        return builder.ToString();
    }

    public static List<string> ToCompactTextualDescriptions(this List<IPluginSignature> plugins)
    {
        return plugins.ConvertAll(plugin => plugin.ToCompactDescription());
    }
}
