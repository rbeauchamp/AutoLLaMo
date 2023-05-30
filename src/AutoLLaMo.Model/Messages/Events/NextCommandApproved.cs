using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Events;

/// <summary>
///     The user approved the command.
/// </summary>
public record NextCommandApproved(NextCommand NextCommand, Message Initiator) : Event(
    Initiator);
