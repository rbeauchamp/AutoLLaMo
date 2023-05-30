using System.Diagnostics;
using AutoLLaMo.Core.Plugins.WriteToFiles;
using AutoLLaMo.Model;
using FluentAssertions;
using Xunit;

namespace AutoLLaMo.Tests.Plugins.WriteToFiles
{
    public class WriteToFilesTests
    {
        [Fact]
        public Task ExecuteAsyncShouldThrowArgumentExceptionWhenCommandIsNotWriteToFiles()
        {
            // Arrange
            var plugin = new WriteToFilesPlugin();
            var command = new FakeCommand();

            // Act & Assert
            return Assert.ThrowsAsync<ArgumentException>(() =>
                plugin.ExecuteAsync(command, CancellationToken.None));
        }

        [Fact]
        public Task ExecuteAsyncShouldThrowInvalidStateExceptionWhenOutputDirectoryEnvironmentVariableIsNotSet()
        {
            // Arrange
            Environment.SetEnvironmentVariable("OutputDirectory", value: null);
            var plugin = new WriteToFilesPlugin();
            var command = new Core.Plugins.WriteToFiles.WriteToFiles
            {
                Contents = new List<FileContent>()
                {
                    new()
                    {
                        FileNameWithExtension = "file.txt",
                        RawContent = "file contents",
                    },
                },
            };

            // Act & Assert
            return Assert.ThrowsAsync<InvalidStateException>(() =>
                plugin.ExecuteAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsyncShouldCreateDirectoryAndWriteFileWhenContentsIsNotEmpty()
        {
            // Arrange
            var plugin = new WriteToFilesPlugin();
            var command = new Core.Plugins.WriteToFiles.WriteToFiles
            {
                Contents = new List<FileContent>()
                {
                    new()
                    {
                        FileNameWithExtension = "file.txt",
                        RawContent = "file contents",
                    },
                },
            };

            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Environment.SetEnvironmentVariable("OutputDirectory", dir);

            // Act
            var response = await plugin.ExecuteAsync(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Output.Should().BeOfType<FileLocations>();
            var fileLocations = response.Output as FileLocations;
            Debug.Assert(
                fileLocations != null,
                nameof(fileLocations) + " != null");
            fileLocations.LocalPaths.Should().ContainSingle();

            fileLocations.LocalPaths[0].Should().Be(Path.Combine(dir, command.Contents[0].FileNameWithExtension));

            var fileExists = File.Exists(fileLocations.LocalPaths[0]);
            fileExists.Should().BeTrue();

            var contents = await File.ReadAllTextAsync(fileLocations.LocalPaths[0]);
            contents.Should().Be(command.Contents[0].RawContent);
        }

        [Fact]
        public async Task ExecuteAsyncShouldCreateMultipleDirectoriesAndWriteMultipleFilesWhenContentsIsNotEmpty()
        {
            // Arrange
            var plugin = new WriteToFilesPlugin();
            var command = new Core.Plugins.WriteToFiles.WriteToFiles
            {
                Contents = new List<FileContent>()
                {
                    new()
                    {
                        FileNameWithExtension = "dir1/file1.txt",
                        RawContent = "dir1 file1 contents",
                    },
                    new()
                    {
                        FileNameWithExtension = "dir2/dir3/file2.txt",
                        RawContent = "dir2 dir3 file2 contents",
                    },
                },
            };

            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Environment.SetEnvironmentVariable("OutputDirectory", dir);

            // Act
            var response = await plugin.ExecuteAsync(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Output.Should().BeOfType<FileLocations>();
            var fileLocations = response.Output as FileLocations;
            Debug.Assert(
                fileLocations != null,
                nameof(fileLocations) + " != null");
            fileLocations.LocalPaths.Should().HaveCount(2);

            fileLocations.LocalPaths[0].Should().Be(Path.Combine(dir, command.Contents[0].FileNameWithExtension));
            fileLocations.LocalPaths[1].Should().Be(Path.Combine(dir, command.Contents[1].FileNameWithExtension));

            var file1Exists = File.Exists(fileLocations.LocalPaths[0]);
            file1Exists.Should().BeTrue();

            var file2Exists = File.Exists(fileLocations.LocalPaths[1]);
            file2Exists.Should().BeTrue();

            var contents1 = await File.ReadAllTextAsync(fileLocations.LocalPaths[0]);
            contents1.Should().Be(command.Contents[0].RawContent);

            var contents2 = await File.ReadAllTextAsync(fileLocations.LocalPaths[1]);
            contents2.Should().Be(command.Contents[1].RawContent);
        }
    }
}
