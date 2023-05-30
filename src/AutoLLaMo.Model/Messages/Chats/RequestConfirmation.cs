namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Request confirmation from a chat participant.
/// </summary>
public record RequestConfirmation(ChatMessage ResponseTo) : ChatResponse(ResponseTo);