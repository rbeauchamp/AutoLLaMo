namespace AutoLLaMo.Core.Plugins.GenerateNewCommand;

public class SourceCodeContent
{
    /// <summary>
    /// The name including extension to use when writing to a file.
    /// </summary>
    public string FileNameWithExtension { get; init; } = string.Empty;

    /// <summary>
    /// The raw content of the C# source code.
    /// </summary>
    public string RawContent { get; init; } = string.Empty;
}
