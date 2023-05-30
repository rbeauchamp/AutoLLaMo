using Rystem.OpenAi;

namespace AutoLLaMo.Model;

/// <summary>
///     Represents the application settings.
/// </summary>
public class Settings
{
    /// <summary>
    ///     The name of the OpenAI language model to use.
    ///     Required.
    /// </summary>
    public ChatModelType OpenAIModel { get; set; } = ChatModelType.Gpt4;

    /// <summary>
    ///     The maximum number of tokens that the model will generate for each response it produces.
    ///     Default is 4,000.
    /// </summary>
    public int MaxTokens { get; set; } = 8_192;

    /// <summary>
    ///     The name of the memory provider.
    ///     Required.
    /// </summary>
    public MemoryProviderName MemoryProvider { get; set; } = MemoryProviderName.Redis;

    /// <summary>
    ///     The name of the AI memory index.
    /// </summary>
    public string MemoryIndexName { get; set; } = "autollm";

    /// <summary>
    ///     The Redis host address.
    ///     Required if <see cref="MemoryProvider" /> is set to <see cref="MemoryProviderName.Redis" />.
    /// </summary>
    public string RedisHost { get; set; } = "localhost";

    /// <summary>
    ///     The Redis port number used for connecting to the Redis server.
    ///     Required if <see cref="MemoryProvider" /> is set to <see cref="MemoryProviderName.Redis" />.
    /// </summary>
    public int RedisPort { get; set; } = 6379;

    public string? RedisPassword { get; set; }

    public string OpenAIApiKey { get; set; } = string.Empty;

    public string? GoogleApiKey { get; set; }

    public string? CustomSearchEngineId { get; set; }
}
