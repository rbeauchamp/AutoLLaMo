namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Request a value from a chat participant.
/// </summary>
public record RequestValue(ChatMessage ResponseTo) : ChatResponse(ResponseTo)
{
    /// <summary>
    ///     The name of the value.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}