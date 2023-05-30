using AutoLLaMo.Model;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace AutoLLaMo.Services.OpenAI
{
    public class OpenAIApi : IOpenAIApi
    {
        private readonly IOpenAiFactory _openAiFactory;
        private readonly IOpenAiUtility _openAiUtility;

        public OpenAIApi(IOpenAiFactory openAiFactory, IOpenAiUtility openAiUtility)
        {
            _openAiFactory = openAiFactory;
            _openAiUtility = openAiUtility;
        }

        public async Task<string> CreateChatCompletionAsync(
            ChatModelType model,
            IReadOnlyList<ChatMessage> chatMessages,
            CancellationToken cancellationToken,
            float temperature = 1.0f,
            int maxTokens = 8_192)
        {
            if (chatMessages.Count == 0)
            {
                throw new InvalidStateException(
                    "The chatMessages parameter must contain at least one chat message.");
            }

            var openAiApi = _openAiFactory.Create();

            ChatRequestBuilder? chatRequest = null;

            foreach (var chatMessage in chatMessages)
            {
                chatRequest = chatRequest == null
                    ? openAiApi.Chat.Request(chatMessage)
                    : chatRequest.AddMessage(chatMessage);
            }

            if (chatRequest == null)
            {
                throw new InvalidStateException("The chatRequest variable must not be null.");
            }

            var actualMaxTokens = maxTokens
                                  - TokenCounter.CountMessageTokens(
                                      chatMessages,
                                      _openAiUtility,
                                      model)
                                  // Reduce by 100 because the token counter is not accurate
                                  - 100;

            var results = await chatRequest
                .WithModel(model)
                .WithTemperature(temperature)
                .WithNumberOfChoicesPerPrompt(value: 1)
                .SetMaxTokens(actualMaxTokens)
                .ExecuteAsync(cancellationToken);

            var content = results.Choices?[index: 0].Message?.Content;

            return content
                   ?? throw new InvalidStateException(
                       "The content from the OpenAI chat completion API must not be null.");
        }
    }
}
