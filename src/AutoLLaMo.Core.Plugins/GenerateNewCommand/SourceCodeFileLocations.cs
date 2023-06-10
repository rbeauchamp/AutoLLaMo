using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.GenerateNewCommand;

/// <summary>
/// A list of full paths to the source code file locations on disk.
/// </summary>
public record SourceCodeFileLocations : Output
{
    /// <summary>
    /// A list of full paths to the source code file locations on disk.
    /// </summary>
    public List<string> LocalPaths { get; init; } = new();

    /// <inheritdoc/>
    public override string Summary { get; init; } = string.Empty;
}