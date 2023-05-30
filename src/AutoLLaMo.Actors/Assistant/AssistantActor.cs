using System.Text.Json;
using System.Text.Json.Serialization;
using AutoLLaMo.Actors.Assistant.Prompts.AssistantRole;
using AutoLLaMo.Actors.Assistant.Prompts.CallToAction;
using AutoLLaMo.Actors.Configurator;
using AutoLLaMo.Actors.Plugin;
using AutoLLaMo.Actors.User;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
using AutoLLaMo.Model.Thoughts;
using AutoLLaMo.Plugins;
using AutoLLaMo.Services.OpenAI;
using Proto;
using Proto.Extensions;
using Rystem.OpenAi.Chat;
using ChatMessage = Rystem.OpenAi.Chat.ChatMessage;

namespace AutoLLaMo.Actors.Assistant
{
    public class AssistantActor : IActor
    {
        private readonly List<ChatMessage> _messageHistory = new();
        private readonly IOpenAIApi _openAIApi;
        private readonly Settings _settings;
        private AssistantConfig? _assistantConfig;

        public AssistantActor(Settings settings, IOpenAIApi openAIApi)
        {
            _settings = settings;
            _openAIApi = openAIApi;
        }

        public async Task ReceiveAsync(IContext context)
        {
            if (context.Parent == null)
            {
                throw new InvalidStateException($"{GetType().Name} requires a parent");
            }

            switch (context.Message)
            {
                case Started:
                    return;
                case ChatWithOpenAI chatWithOpenAI:
                    await ChatWithOpenAIAsync(context, chatWithOpenAI);
                    return;
                case NextCommandApproved:
                    context.Forward<PluginActor>();
                    return;
                case NextCommandExecuted nextCommandExecuted:
                    context.Send<UserActor>(nextCommandExecuted);
                    return;
                case RequestValue requestValue:
                    context.Send<UserActor>(requestValue);
                    return;
                case ConfigureAssistant:
                case ProvideValue:
                    context.Forward<ConfiguratorActor>();
                    return;
                case AssistantConfigured assistantConfigured:
                    _assistantConfig = assistantConfigured.AssistantConfig;
                    context.Send<UserActor>(assistantConfigured);
                    return;
                default:
                    throw new InvalidStateException(
                        $"{GetType().Name} cannot handle {context.Message.GetMessageTypeName()}");
            }
        }

        /// <summary>
        /// Chat with OpenAI.
        /// </summary>
        /// <param name="actorContext">The actor context.</param>
        /// <param name="chatWithOpenAI">The message that initiated the chat with OpenAI.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="AutoLLaMo.Model.InvalidStateException">The {nameof(AssistantConfig)} must not be null.</exception>
        /// <exception cref="System.InvalidOperationException">Could not deserialize the assistant response: {assistantResponseJson}</exception>
        private async Task ChatWithOpenAIAsync(
            IContext actorContext,
            ChatWithOpenAI chatWithOpenAI)
        {
            if (_assistantConfig == null)
            {
                throw new InvalidStateException($"The {nameof(AssistantConfig)} must not be null.");
            }

            var chatMessages = new List<ChatMessage>();

            // This should go first as it provides instructions to the assistant
            var assistantRolePrompt = _assistantConfig.GenerateAssistantRolePrompt();
            chatMessages.Add(assistantRolePrompt);

            // Then the message history should be added next as they provide context, i.e. conversation history, to the assistant
            chatMessages.AddRange(_messageHistory);

            // Then add the command output message, if any
            ChatMessage? nextCommandOutputMessage = null;
            if (chatWithOpenAI.NextCommandExecuted is { } nextCommandExecuted)
            {
                var nextCommandOutput = nextCommandExecuted.Response.ToJson();

                nextCommandOutputMessage = new ChatMessage
                {
                    Role = ChatRole.System,
                    Content =
                        $@"Command {nextCommandExecuted.NextCommandApproved.NextCommand.Name} returned:
{nextCommandOutput}
",
                };

                chatMessages.Add(nextCommandOutputMessage);
            }

            // The call to action should go last as it prompts the assistant to respond
            var callToActionPrompt = CallToActionPromptUtil.GenerateCallToActionPrompt();
            chatMessages.Add(callToActionPrompt);

            var assistantResponseJson = await _openAIApi.CreateChatCompletionAsync(
                _settings.OpenAIModel,
                chatMessages,
                actorContext.CancellationToken,
                maxTokens: _settings.MaxTokens);

            var assistantResponse = JsonSerializer.Deserialize<AssistantResponse>(
                                        assistantResponseJson,
                                        new JsonSerializerOptions
                                        {
                                            WriteIndented = true,
                                            DefaultIgnoreCondition =
                                                JsonIgnoreCondition.WhenWritingNull,
                                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                        })
                                    ?? throw new InvalidOperationException(
                                        $"Could not deserialize the assistant response: {assistantResponseJson}");

            // Add the user's call to action prompt and the assistant's response to the message history
            if (nextCommandOutputMessage != null)
            {
                _messageHistory.Add(nextCommandOutputMessage);
            }

            _messageHistory.Add(callToActionPrompt);
            _messageHistory.Add(
                new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Content = assistantResponseJson,
                });

            actorContext.Send<UserActor>(new AssistantResponded(assistantResponse, chatWithOpenAI));
        }
    }
}
