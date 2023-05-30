using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Represents a message exchanged between participants in a chat.
/// </summary>
public abstract record ChatMessage : Message
{
    /// <summary>
    ///     The text of the message, parsed into lines.
    /// </summary>
    public IList<string>? Lines { get; init; }
}
