using AutoLLaMo.Plugins;

namespace AutoLLaMo.Actors.Assistant.Prompts.AssistantRole;

public class AssistantRolePromptContent
{
    public string Role { get; init; } = string.Empty;
    public List<string> Goals { get; init; } = new();
    public string ResponseDirective { get; init; } = string.Empty;
    public string UsefulContext { get; init; } = string.Empty;
    public List<string> Constraints { get; init; } = new();
    public List<string> Important { get; init; } = new();
    public List<IPluginSignature> AvailableCommands { get; init; } = new();
    public List<string> PerformanceEvaluationCriteria { get; init; } = new();
    public string ResponseFormat { get; init; } = string.Empty;
}
