namespace AutoLLaMo.Model;

public record Arg
{
    /// <summary>
    ///     The arg name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     The arg value.
    /// </summary>
    public string? Value { get; init; }
}
