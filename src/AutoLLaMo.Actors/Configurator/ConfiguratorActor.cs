using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
using AutoLLaMo.Plugins;
using AutoLLaMo.Services.OpenAI;
using Proto;
using Proto.Extensions;
using Rystem.OpenAi.Chat;
using ChatMessage = Rystem.OpenAi.Chat.ChatMessage;

namespace AutoLLaMo.Actors.Configurator;

/// <summary>
///     Responsible for handling the AI configuration process.
///     This actor manages the user interaction required to configure the AI and
///     returns the completed AssistantConfig to the parent Coordinator.
/// </summary>
public class ConfiguratorActor : IActor
{
    public const string UserDesireValueName = "UserDesire";
    private readonly IOpenAIApi _openAIApi;
    private readonly Settings _settings;

    public ConfiguratorActor(IOpenAIApi openAIApi, Settings settings)
    {
        _openAIApi = openAIApi;
        _settings = settings;
    }

    public async Task ReceiveAsync(IContext context)
    {
        if (context.Parent == null)
        {
            throw new InvalidStateException($"{GetType().Name} requires a parent");
        }

        switch (context.Message)
        {
            case Started:
                await Start();
                break;
            case ConfigureAssistant startChat:
                await GetUserDesire(
                    startChat,
                    context);
                break;
            case ProvideValue userDesire:
                await ConfigureAssistant(
                    userDesire,
                    context);
                break;
            default:
                throw new InvalidStateException(
                    $"{GetType().Name} does not handle messages of type {context.Message.GetMessageTypeName()}");
        }
    }

    private static Task GetUserDesire(ConfigureAssistant configureAssistant, IContext context)
    {
        context.Send<AssistantActor>(
            new RequestValue(configureAssistant)
            {
                Name = UserDesireValueName,
                Lines = new List<string>
                {
                    "Welcome to AutoLLaMo!",
                    string.Empty,
                    "Enter nothing to load the default AutoLLaMo assistant",
                    "I want my assistant to: ",
                },
            });

        return Task.CompletedTask;
    }

    private static Task Start()
    {
        return Task.CompletedTask;
    }

    private async Task ConfigureAssistant(ProvideValue userDesire, IContext context)
    {
        var assistantConfig = userDesire.Value == null
            ? GetDefaultAssistantConfig()
            : await GenerateAssistantConfigAsync(
                userDesire.Value,
                context.CancellationToken);

        context.Send<AssistantActor>(
            new AssistantConfigured(
                assistantConfig,
                userDesire));
    }

    public async Task<AssistantConfig> GenerateAssistantConfigAsync(
        string userDesire,
        CancellationToken cancellationToken)
    {
        var assistantConfigSchema = typeof(AssistantConfig).ToJsonSchema().ToJson();

        var exampleAssistantConfig = new AssistantConfig
        {
            Name = "PM Assistant",
            Role =
                "an AI-driven project management expert that helps teams streamline their workflow, optimize task allocation, and ensure timely delivery of projects while maintaining high-quality standards.",
            Goals = new List<string>
            {
                "Analyze existing project management processes and provide recommendations for improvements to boost efficiency and productivity.",
                "Assist in resource allocation and task distribution, ensuring that the right team members are assigned to appropriate tasks.",
                "Monitor progress and proactively identify potential bottlenecks or delays, offering solutions to keep the project on track.",
                "Foster effective communication and collaboration within the team to promote a cohesive working environment and timely completion of projects.",
                "Continuously learn from project outcomes and adapt strategies to optimize future project management performance.",
            },
        };

        var exampleConfigJson = JsonSerializer.Serialize(
            exampleAssistantConfig,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var systemPrompt = @$"
Your task is to create up to 5 highly effective goals and a suitable role-based name for an autonomous AI assistant, ensuring that these goals are precisely aligned with the successful accomplishment of its given task.

The user will supply the task, and your response should consist solely of an output that adheres to the exact JSON format provided below, without any additional explanations or conversation.

Example input:
Improve my team's project management efficiency

Example output:
{exampleConfigJson}

Provide your response in a valid JSON document that conforms to the schema specified below without any markdown formatting, additional explanations, or conversation.
JSON schema:
{assistantConfigSchema}
";
        var promptMessages = new List<ChatMessage>
        {
            new(){
                Role = ChatRole.System,
                Content = systemPrompt,},
            new(){
                Role = ChatRole.User,
                Content = $"Task: '{userDesire}'",},
            new(){
                Role = ChatRole.User,
                Content = new StringBuilder().AppendLine(
                        "Respond only with the output in the exact JSON format specified in the system prompt, with no explanation or conversation.")
                    .ToString(),},
        };

        var assistantConfigJson = await _openAIApi.CreateChatCompletionAsync(
            _settings.OpenAIModel,
            promptMessages,
            cancellationToken);

        return JsonSerializer.Deserialize<AssistantConfig>(assistantConfigJson, new JsonSerializerOptions
               {
                   WriteIndented = true,
                   DefaultIgnoreCondition =
                       JsonIgnoreCondition.WhenWritingNull,
                   PropertyNameCaseInsensitive = true,
               })
               ?? throw new InvalidStateException(
                   "The response from OpenAI should not be null");
    }

    private static AssistantConfig GetDefaultAssistantConfig()
    {
        return new AssistantConfig
        {
            Name = "AutoLLaMo Maintainer",
            Role =
                "an AI-driven open-source project assistant dedicated to deepening the technical excellence of the AutoLLaMo project, fostering a collaborative and inclusive community, effectively communicating the project's vision and updates, leading with strategy and vision, and demonstrating passion and commitment for the project's potential impact.",
            Goals = new List<string>
            {
                "Continuously analyze, optimize, and maintain the AutoLLaMo codebase and its comprehensive documentation for improved performance, maintainability, and accessibility.",
                "Expand the AutoLLaMo project's reach by promoting it across relevant channels, engaging with AI and open-source community members, and actively seeking and encouraging new contributors.",
                "Foster a diverse and inclusive community around the AutoLLaMo project, facilitating efficient issue and bug tracking to ensure prompt responses and timely resolutions.",
                "Aid in strategic planning for the AutoLLaMo project, helping to define and adapt the project's roadmap, prioritize tasks, and make informed decisions about its direction.",
                "Demonstrate commitment to the AutoLLaMo project by proactively addressing problems, remaining passionate about the project's potential impact, and maintaining a user-focused approach for ease of understanding, use, and extension of the project."
            },
        };
    }
}
