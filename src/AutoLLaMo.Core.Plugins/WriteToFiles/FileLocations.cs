using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.WriteToFiles
{
    /// <summary>
    /// A list of full paths to the file locations on disk.
    /// </summary>
    public record FileLocations : Output
    {
        /// <summary>
        /// A list of full paths to the file locations on disk.
        /// </summary>
        public List<string> LocalPaths { get; init; } = new();

        /// <inheritdoc/>
        public override string Summary { get; init; } = string.Empty;
    }
}
