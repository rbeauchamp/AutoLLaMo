using System.ComponentModel.DataAnnotations;
using AutoLLaMo.Common;
using AutoLLaMo.Plugins;
using LibGit2Sharp;

namespace AutoLLaMo.Core.Plugins.CloneRepository
{
    /// <summary>
    ///     Plugin to clone a repository.
    /// </summary>
    public class CloneRepositoryPlugin : Plugin
    {
        /// <inheritdoc />
        public override IPluginSignature Signature =>
            new PluginSignature<CloneRepository, CloneRepositoryOutput>();

        public override Task<Response> ExecuteAsync(
            Command command,
            CancellationToken cancellationToken)
        {
            if (command is not CloneRepository cloneRepository)
            {
                throw new ArgumentException(
                    "Command is not a valid CloneRepository command",
                    nameof(command));
            }

            var (isValid, validationResults) = cloneRepository.Validate();

            if (!isValid)
            {
                return Task.FromResult(
                    new Response
                    {
                        Output = null,
                        Errors = validationResults,
                    });
            }

            var outputDirectory = Environment.GetEnvironmentVariable("OutputDirectory")
                                  ?? Path.GetTempPath();
            var gitUrl = cloneRepository.RepositoryUrl;
            var localRootDirectory = cloneRepository.WorkingDirectoryName ?? Path.GetFileNameWithoutExtension(gitUrl);
            var localRepoPath = Path.Combine(
                outputDirectory,
                localRootDirectory);

            try
            {
                Repository.Clone(
                    cloneRepository.RepositoryUrl,
                    localRepoPath);
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    new Response
                    {
                        Output = null,
                        Errors = new List<ValidationResult>
                        {
                            new(
                                $"Failed to clone repository: {ex.Message}",
                                new[]
                                {
                                    nameof(CloneRepository.RepositoryUrl),
                                }),
                        },
                    });
            }

            return Task.FromResult(
                new Response
                {
                    Output = new CloneRepositoryOutput
                    {
                        LocalPath = localRepoPath,
                    },
                });
        }
    }
}
