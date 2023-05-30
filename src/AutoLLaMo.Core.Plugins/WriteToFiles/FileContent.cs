namespace AutoLLaMo.Core.Plugins.WriteToFiles;

/// <summary>
/// The name and content of a file.
/// </summary>
public class FileContent
{
    /// <summary>
    /// The file name including extension.
    /// </summary>
    public string FileNameWithExtension { get; init; } = string.Empty;

    /// <summary>
    /// The raw content of the file.
    /// </summary>
    public string RawContent { get; init; } = string.Empty;
}
