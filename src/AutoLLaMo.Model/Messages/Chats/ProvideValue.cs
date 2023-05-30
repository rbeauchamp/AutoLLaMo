namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Provide a requested value.
/// </summary>
public record ProvideValue(ChatMessage ResponseTo) : ChatResponse(ResponseTo)
{
    /// <summary>
    ///     The requested value.
    /// </summary>
    public string? Value { get; init; }
}