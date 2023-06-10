using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace AutoLLaMo.Services.OpenAI;

public static class TokenCounter
{
    public static int CountMessageTokens(
        IEnumerable<ChatMessage> messages,
        IOpenAiUtility openAiUtility,
        ChatModelType model)
    {
        return messages.Select(message => message.Content)
            .Where(content => !string.IsNullOrWhiteSpace(content)).Sum(
                content => CountStringTokens(
                    content,
                    openAiUtility,
                    model));
    }

    private static int CountStringTokens(
        this string? content,
        IOpenAiUtility openAiUtility,
        ChatModelType modelName)
    {
        return openAiUtility.Tokenizer.WithChatModel(modelName).Encode(content).NumberOfTokens;
    }
}