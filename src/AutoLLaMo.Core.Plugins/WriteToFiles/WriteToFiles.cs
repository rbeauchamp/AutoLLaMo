using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.WriteToFiles
{
    /// <summary>
    /// Writes a list of content to files on disk.
    /// </summary>
    public record WriteToFiles : Command
    {
        /// <summary>
        /// A list of content to write to disk.
        /// </summary>
        public List<FileContent> Contents { get; init; } = new();
    }
}
