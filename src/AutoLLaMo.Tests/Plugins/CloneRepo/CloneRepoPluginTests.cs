using System.Diagnostics;
using AutoLLaMo.Core.Plugins.CloneRepository;
using FluentAssertions;
using Xunit;

namespace AutoLLaMo.Tests.Plugins.CloneRepo
{
    public class CloneRepoPluginTests
    {
        private readonly CloneRepositoryPlugin _plugin;

        public CloneRepoPluginTests()
        {
            _plugin = new CloneRepositoryPlugin();
        }

        [Fact]
        public async Task CloneRepoValidUrlClonesRepoAndReturnsLocalPath()
        {
            // Arrange
            const string validRepoUrl = "https://github.com/octocat/Spoon-Knife.git";
            var cloneRepository = new CloneRepository
            {
                RepositoryUrl = validRepoUrl,
                WorkingDirectoryName = Guid.NewGuid().ToString(),
            };

            // Act
            var response = await _plugin.ExecuteAsync(
                cloneRepository,
                CancellationToken.None);

            // Assert
            response.Errors.Should().BeEmpty();
            response.Output.Should().BeOfType<CloneRepositoryOutput>();
            var output = response.Output as CloneRepositoryOutput;
            Debug.Assert(
                output != null,
                nameof(output) + " != null");
            output.LocalPath.Should().Contain(cloneRepository.WorkingDirectoryName);
            Directory.Exists(output.LocalPath).Should().BeTrue();
        }

        [Fact]
        public async Task CloneRepoInvalidUrlReturnsValidationErrors()
        {
            // Arrange
            const string invalidRepoUrl = "invalid_url";
            var cloneRepository = new CloneRepository
            {
                RepositoryUrl = invalidRepoUrl,
                WorkingDirectoryName = Guid.NewGuid().ToString(),
            };

            // Act
            var response = await _plugin.ExecuteAsync(
                cloneRepository,
                CancellationToken.None);

            // Assert
            response.Errors.Should().NotBeNull();
            response.Output.Should().BeNull();
        }
    }
}
