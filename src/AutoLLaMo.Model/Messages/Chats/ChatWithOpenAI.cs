using AutoLLaMo.Model.Messages.Events;

namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Chat with the large language model.
/// </summary>
public record ChatWithOpenAI(NextCommandExecuted? NextCommandExecuted) : ChatMessage;
