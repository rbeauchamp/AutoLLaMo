using AutoLLaMo.Model.Messages.Events;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Chat with the large language model.
/// </summary>
public record ChatWithOpenAI(NextCommandExecuted? NextCommandExecuted) : Message;
