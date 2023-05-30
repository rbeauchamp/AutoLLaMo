using System.ComponentModel.DataAnnotations;

namespace AutoLLaMo.Plugins;

public record Response
{
    /// <summary>
    /// The errors that prevented the command from executing.
    /// </summary>
    public List<ValidationResult> Errors { get; init; } = new();

    /// <summary>
    /// The output of the command.
    /// </summary>
    public Output? Output { get; init; }
}
