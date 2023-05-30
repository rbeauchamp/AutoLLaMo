using AutoLLaMo.Model.Thoughts;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Events;

public record AssistantResponded(AssistantResponse AssistantResponse, Message Initiator) : Event(
    Initiator);
