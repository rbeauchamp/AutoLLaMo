using AutoLLaMo.Model;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.WriteToFiles
{
    public class WriteToFilesPlugin : Plugin
    {
        public override IPluginSignature Signature { get; } = new PluginSignature<WriteToFiles, FileLocations>();

        public override async Task<Response> ExecuteAsync(
            Command command,
            CancellationToken cancellationToken)
        {
            if (command is not WriteToFiles writeFiles)
            {
                throw new ArgumentException(
                    "Invalid command type.",
                    nameof(command));
            }

            var outputPath = Environment.GetEnvironmentVariable("OutputDirectory");

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new InvalidStateException("OutputDirectory environment variable is not set.");
            }

            var localPaths = new List<string>();

            foreach (var file in writeFiles.Contents)
            {
                var filePath = Path.Combine(outputPath, file.FileNameWithExtension);

                // Create the directory if it doesn't exist.
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidStateException("The directory name must not be null."));

                await File.WriteAllTextAsync(filePath, file.RawContent, cancellationToken);

                localPaths.Add(filePath);
            }

            return new Response
            {
                Output = new FileLocations
                {
                    LocalPaths = localPaths,
                },
            };
        }
    }
}
