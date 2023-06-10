using System.ComponentModel.DataAnnotations;
using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.CloneRepository;

/// <summary>
/// Clones a GitHub repository.
/// </summary>
public record CloneRepository : Command
{
    /// <summary>
    /// URL of the repository to be cloned.
    /// </summary>
    [Required]
    [StringLength(2000, MinimumLength = 3)]
    public string RepositoryUrl { get; init; } = string.Empty;

    /// <summary>
    /// An optional working directory name to clone the repository into.
    /// </summary>
    [StringLength(255, MinimumLength = 1)]
    public string? WorkingDirectoryName { get; init; }
}