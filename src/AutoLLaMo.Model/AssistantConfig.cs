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
    /// </summary>
    public List<string> Goals { get; init; } = new();
}
