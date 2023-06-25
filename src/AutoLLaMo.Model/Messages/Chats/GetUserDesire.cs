using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Get the user's desire to be fulfilled by an assistant.
/// </summary>
public record GetUserDesire(Message ResponseTo) : ChatResponse(ResponseTo);
