using AutoLLaMo.Model;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace AutoLLaMo.Services.OpenAI;

/// <summary>
/// Defines the methods that a service interacting with the OpenAI API must implement.
/// </summary>
public interface IOpenAIApi
{
    /// <summary>
    /// Asynchronously creates a chat completion using the OpenAI API.
    /// </summary>
    /// <param name="model">The language model to use for the chat completion.</param>
    /// <param name="chatMessages">A list of chat messages to be processed by the language model.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <param name="temperature">Controls randomness of the output. Default value is 1.0.</param>
    /// <param name="maxTokens">The maximum number of tokens in the output. If not specified, the API's default is used.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the content of the chat message from the language model's output.</returns>
    /// <exception cref="InvalidStateException">Thrown when the API response is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the maximum number of retries is exceeded.</exception>
    Task<string> CreateChatCompletionAsync(
        ChatModelType model,
        IReadOnlyList<ChatMessage> chatMessages,
        CancellationToken cancellationToken,
        float temperature = 1.0f,
        int maxTokens = 8_192);
}
