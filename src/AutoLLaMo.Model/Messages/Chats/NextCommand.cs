namespace AutoLLaMo.Model.Messages.Chats;

public record NextCommand
{
    /// <summary>
    /// Name of the command to execute.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// JSON that can be deserialized into the next command.
    /// </summary>
    public string Json { get; init; } = string.Empty;
}
