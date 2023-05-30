using System.Text.Json;
using System.Text.Json.Serialization;
using AutoLLaMo.Model;
using AutoLLaMo.Plugins;
using AutoLLaMo.Services.OpenAI;
using Rystem.OpenAi.Chat;

namespace AutoLLaMo.Core.Plugins.GenerateNewCommand
{
    public class GenerateNewCommandPlugin : Plugin
    {
        private readonly IOpenAIApi _openAIApi;
        private readonly Settings _settings;

        public GenerateNewCommandPlugin(IOpenAIApi openAIApi, Settings settings)
        {
            _openAIApi = openAIApi;
            _settings = settings;
        }

        public override IPluginSignature Signature { get; } = new PluginSignature<GenerateNewCommand, SourceCodeFileLocations>();

        public override async Task<Response> ExecuteAsync(
            Command command,
            CancellationToken cancellationToken)
        {
            if (command is not GenerateNewCommand generateNewCommand)
            {
                throw new ArgumentException(
                    "Invalid command type.",
                    nameof(command));
            }

            var systemPrompt = @$"
You are the abstract C# Plugin class, tasked with generating several implementations when provided an instance of GenerateNewCommand. Here are your responsibilities:

1. Create an implementation of Command as specified in GenerateNewCommand.
2. Craft a Response.Output implementation based on DetailedDecriptionOfOutput.
3. Self-replicate with fully-implemented C# functionality that executes the new command and returns the new output.
4. Construct an xUnit test fixture composed of Facts to validate the main flow and all logical edge cases.
5. Ensure the code is fully implemented and functional such that all unit tests will pass, with no TODOs, stubs, placeholders, or `throw new NotImplementedException()`.

Your work must adhere to these coding standards:

1. Ensure every new non-nullable command property is initialized with non-null values at declaration to support nullable reference types.
2. Extract any needed settings or configurations from environment variables.
3. Maintain environment variable names in PascalCase.
4. Ensure every new command property has appropriate validation attributes, setting reasonable minimum and maximum limits; e.g., `[Range(3, 100)]` for numbers and `[StringLength(100, MinimumLength = 3)]` for strings.
5. The new plugin should check the command type and validate attributes using FluentValidation. Return any errors in the ValidationErrors property of the response.
6. Use FluentAssertions for all xUnit assertions.
7. Segregate each xUnit test into // Arrange, // Act, // Assert sections.

Here is your definition:
-----
/// <summary>
/// Defines a plugin that executes a command.
/// </summary>
public abstract class Plugin
{{
    /// <summary>
    /// The signature that this plugin supports.
    /// </summary>
    public abstract IPluginSignature Signature {{ get; }}

    /// <summary>
    /// Implements custom logic for executing a command
    /// </summary>
    /// <param name=""command"">The command to execute.</param>
    /// <param name=""cancellationToken"">The cancellation token.</param>
    public abstract Task<Response> ExecuteAsync(
        Command command,
        CancellationToken cancellationToken);
}}
-----

And here are your dependencies:
-----
using System.ComponentModel.DataAnnotations;

/// <summary>
///     Base implementation of a plugin's signature.
/// </summary>
/// <typeparam name=""TCommand"">The command type.</typeparam>
/// <typeparam name=""TOutput"">The output type.</typeparam>
public record PluginSignature<TCommand, TOutput> : IPluginSignature
    where TCommand : Command
    where TOutput : Output
{{
    /// <summary>
    ///     Defines the command that the plugin will execute.
    /// </summary>
    public Type CommandType => typeof(TCommand);

    /// <summary>
    ///     Defines the output that executing the command will produce.
    /// </summary>
    public Type OutputType => typeof(TOutput);
}}

public record Response
{{
    /// <summary>
    /// The errors that prevented the command from executing.
    /// </summary>
    public List<ValidationResult> Errors {{ get; init; }} = new();

    /// <summary>
    /// The output of the command.
    /// </summary>
    public Output? Output {{ get; init; }}
}}

/// <summary>
/// Represents a request or intent to alter the state of the system.
/// </summary>
public abstract record Command : Message
{{
}}

/// <summary>
/// The result of executing a command.
/// </summary>
public abstract record Output
{{
    /// <summary>
    /// A one sentence summary of what was produced.
    /// </summary>
    public abstract string Summary {{ get; init; }}
}}
-----

Provide your response in a valid JSON document, with all special characters and interpolated strings correctly escaped, that conforms to the schema specified below without any markdown formatting, additional explanations, or conversation. Your response must be deserializable by System.Text.Json.JsonSerializer.
JSON schema:
{typeof(SourceCode).ToJsonSchema().ToJson()}
";

            var generateNewCommandJson = JsonSerializer.Serialize(
                generateNewCommand,
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                });

            var userPrompt = $@"
Please generate a new command, output, plugin, and tests based on the following GenerateNewCommand instance, and respond using the format specified above:
{generateNewCommandJson}
";

            var promptMessages = new List<ChatMessage>
            {
                new()
                {
                    Role = ChatRole.System,
                    Content = systemPrompt,
                },
                new()
                {
                    Role = ChatRole.User,
                    Content = userPrompt,
                },
            };

            var jsonString = await _openAIApi.CreateChatCompletionAsync(
                _settings.OpenAIModel,
                promptMessages,
                cancellationToken);

            var sourceCode = JsonSerializer.Deserialize<SourceCode>(jsonString, new JsonSerializerOptions
                               {
                                   WriteIndented = true,
                                   DefaultIgnoreCondition =
                                       JsonIgnoreCondition.WhenWritingNull,
                                   PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                               })
                               ?? throw new InvalidStateException(
                                   "The response from OpenAI should not be null");

            return await WriteToFilesAsync(sourceCode, cancellationToken);
        }

        private static async Task<Response> WriteToFilesAsync(
            SourceCode sourceCode,
            CancellationToken cancellationToken)
        {
            var outputPath = Environment.GetEnvironmentVariable("OutputDirectory");

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new InvalidStateException("OutputDirectory environment variable is not set.");
            }

            var localPaths = new List<string>();

            foreach (var file in sourceCode.SourceCodeContents)
            {
                var filePath = Path.Combine(outputPath, file.FileNameWithExtension);

                // Create the directory if it doesn't exist.
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidStateException("The directory name must not be null."));

                await File.WriteAllTextAsync(filePath, file.RawContent, cancellationToken);

                localPaths.Add(filePath);
            }

            return new Response
            {
                Output = new SourceCodeFileLocations
                {
                    LocalPaths = localPaths,
                    Summary = sourceCode.Summary.Replace(".", string.Empty) + " and wrote the source code files to disk. The code has not been added to the AutoLLaMo repository.",
                },
            };
        }
    }
}
