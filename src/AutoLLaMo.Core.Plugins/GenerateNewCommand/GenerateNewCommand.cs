using AutoLLaMo.Plugins;

namespace AutoLLaMo.Core.Plugins.GenerateNewCommand;

/// <summary>
/// Generates a simple, granular command as specified in the arguments. Ensure the command adheres to the single-responsibility principle, has generic inputs, and does not duplicate existing commands.
/// </summary>
/// <example>
/// Examples include "BuildDockerContainer" using Docker.DotNet, "PostTweet" using Tweetinvi, "SearchGoogle" using  Google.Apis.CustomSearchAPI.v1, "ScrapeWebpage" using HtmlAgilityPack.
/// </example>
public record GenerateNewCommand : Command
{
    /// <summary>
    /// Name of the new command.
    /// </summary>
    public string NewCommandName { get; init; } = string.Empty;

    /// <summary>
    /// NuGet package to implement the new command.
    /// </summary>
    public string NuGetPackageName { get; init; } = string.Empty;

    /// <summary>
    /// New command's purpose for aiding LLM AI.
    /// </summary>
    public string Justification { get; init; } = string.Empty;

    /// <summary>
    /// A description of what should be produced when executing this command.
    /// </summary>
    public string OutputDescription { get; init; } = string.Empty;

    /// <summary>
    /// Required arguments for the new command.
    /// </summary>
    public string CommaSeparatedListOfRequiredArgs { get; init; } = string.Empty;

    /// <summary>
    /// Optional arguments for the new command.
    /// </summary>
    public string CommaSeparatedListOfOptionalArgs { get; init; } = string.Empty;
}
