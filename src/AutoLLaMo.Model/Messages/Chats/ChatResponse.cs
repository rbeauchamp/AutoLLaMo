using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Represent a response to a message.
/// </summary>
/// <param name="ResponseTo">
///     The chat message that this is a response to.
/// </param>
public abstract record ChatResponse(Message ResponseTo) : Message;
