namespace AutoLLaMo.Model;

public class AssistantConfig
{
    /// <summary>
    ///     The name of the assistant.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     The description of the assistant.
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    ///     The set of goals the assistant will work to achieve.
    ///     See https://chat.openai.com/c/1a4d005c-9c8f-444d-8a1f-f149826e7274 for possible enhancements.
    /// </summary>
    public List<string> Goals { get; init; } = new();
}
