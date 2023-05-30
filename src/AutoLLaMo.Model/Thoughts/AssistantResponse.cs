using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Thoughts;

public record AssistantResponse : Message
{
    /// <summary>
    /// Represents your thought process, encompassing thoughts, reasoning, a plan of action, and a critique of the plan
    /// </summary>
    public ThoughtProcess ThoughtProcess { get; init; } = new();

    /// <summary>
    /// The next command to execute
    /// </summary>
    public NextCommand NextCommand { get; init; } = new();
}
