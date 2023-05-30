namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Provide a confirmation to a chat message.
/// </summary>
public record Confirm(ChatMessage ResponseTo) : ChatResponse(ResponseTo)
{
    /// <summary>
    ///     Whether the message is confirmed.
    /// </summary>
    public bool IsConfirmed { get; init; }
}