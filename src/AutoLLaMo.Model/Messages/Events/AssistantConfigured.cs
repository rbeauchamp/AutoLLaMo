using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Events;

public record AssistantConfigured(AssistantConfig AssistantConfig, Message Initiator) : Event(
    Initiator);
