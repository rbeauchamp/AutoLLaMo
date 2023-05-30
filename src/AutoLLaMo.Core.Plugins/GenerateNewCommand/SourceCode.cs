using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.GenerateNewCommand
{
    /// <summary>
    /// A list of C# source code content.
    /// </summary>
    public record SourceCode : Output
    {
        /// <summary>
        /// A list of C# source code content.
        /// </summary>
        public List<SourceCodeContent> SourceCodeContents { get; init; } = new();

        /// <inheritdoc />
        public override string Summary { get; init; } = string.Empty;
    }
}
