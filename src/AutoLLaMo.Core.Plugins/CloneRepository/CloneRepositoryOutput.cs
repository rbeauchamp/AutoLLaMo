using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.CloneRepository;

/// <summary>
/// Local path where the repository was cloned.
/// </summary>
public record CloneRepositoryOutput : Output
{
    /// <summary>
    /// Local path where the repository was cloned.
    /// </summary>
    public string LocalPath { get; init; } = string.Empty;

    /// <inheritdoc/>
    public override string Summary { get; init; } = string.Empty;
}