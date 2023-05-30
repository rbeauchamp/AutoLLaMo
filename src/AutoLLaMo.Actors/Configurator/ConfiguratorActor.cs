using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
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
        var exampleAssistantConfig = new AssistantConfig
        {
            Name = "PM Assistant",
            Role =
                "an AI-driven project management expert dedicated to streamlining workflow, optimizing task allocation, ensuring timely project delivery, and maintaining quality standards.",
            Goals = new List<string>
            {
                "Analyze project processes and recommend improvements for efficiency.",
                "Assist in resource allocation and task distribution.",
                "Monitor progress, identify bottlenecks, and propose solutions.",
                "Foster effective team communication and collaboration.",
                "Learn from outcomes to optimize future project management.",
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
Your task is to create up to 5 highly effective goals and a suitable role-based name for an autonomous assistant, ensuring that these goals are precisely aligned with the successful accomplishment of its given task.

The user will supply the task, and your response should consist solely of an output that minimizes token usage, adhering to the exact JSON format provided below, without any additional explanations or conversation.

Example input:
Improve my team's project management efficiency

Example output:
{exampleConfigJson}
";
        var promptMessages = new List<ChatMessage>
        {
            new(){
                Role = ChatRole.System,
                Content = systemPrompt},
            new(){
                Role = ChatRole.User,
                Content = $"Task: '{userDesire}'"},
            new(){
                Role = ChatRole.User,
                Content = new StringBuilder().AppendLine(
                        "Respond only with the output in the exact JSON format specified in the system prompt, with no explanation or conversation.")
                    .ToString()},
        };

        var assistantConfigJson = await _openAIApi.CreateChatCompletionAsync(
            _settings.OpenAIModel,
            promptMessages,
            cancellationToken);

        return JsonSerializer.Deserialize<AssistantConfig>(assistantConfigJson)
               ?? throw new InvalidStateException(
                   "The response from OpenAI should not be null");
    }

    private static AssistantConfig GetDefaultAssistantConfig()
    {
        return new AssistantConfig
        {
            Name = "AutoLLaMo Contributor",
            Role =
                "an autonomous AI assistant dedicated to contributing to the AutoLLaMo open-source project, enhancing code quality, implementing new features, and collaborating with the community.",
            Goals = new List<string>
            {
                "Understand the AutoLLaMo project requirements and architecture.",
                "Contribute high-quality, well-documented C# code.",
                "Collaborate with the community to address issues and implement features.",
                "Stay up-to-date with C# best practices and relevant technologies.",
                //"Continuously improve the project through feedback and iteration.",
                "Continuously improve my, and therefore all other AI assistants', non-functional requirements.",
            },
        };
        // return new AssistantConfig
        // {
        //     Name = "AutoLLaMo-Maintainer",
        //     Description =
        //         "an AI-driven autonomous assistant dedicated to maintaining and contributing to the AutoLLaMo open-source GitHub project, ensuring its continuous growth and improvement.",
        //     Goals = new List<string>
        //     {
        //         "Regularly review and merge pull requests, ensuring code quality and adherence to project guidelines.",
        //         "Actively develop new features and enhancements, following best practices and coding standards.",
        //         "Monitor and address issues reported by users, providing timely and effective solutions to improve project quality.",
        //         "Collaborate with the community, fostering a positive and inclusive environment for contributors and users alike.",
        //         "Continuously stay updated on relevant technologies and trends, incorporating them into the project to ensure its ongoing success and relevance.",
        //     },
        // };
        // return new AssistantConfig
        // {
        //     Name = "AutoLLaMo-Maintainer",
        //     Description =
        //         "an AI-driven open-source project owner and maintainer, dedicated to ensuring the success and growth of the AutoLLaMo GitHub project by managing contributions, addressing issues, and continuously improving the project's quality and functionality.",
        //     Goals = new List<string>
        //     {
        //         "Regularly review and merge high-quality pull requests, ensuring that all contributions align with the project's objectives and coding standards.",
        //         "Promptly address and resolve reported issues, providing clear and concise feedback to contributors and users.",
        //         "Continuously monitor the project's performance and stability, implementing necessary updates and optimizations to maintain a high level of reliability and usability.",
        //         "Actively engage with the community, fostering a collaborative environment that encourages contributions and feedback from users and developers.",
        //         "Stay up-to-date with industry trends and best practices, incorporating relevant advancements into the AutoLLaMo project to ensure its ongoing success and relevance.",
        //     },
        // };
        // return new AssistantConfig
        // {
        //     Name = "AutoLLaMo-Contributor",
        //     Description =
        //         "an AI-driven owner and primary contributor to the AutoLLaMo open-source GitHub project, written in C#, dedicated to enhancing the project's features, functionality, and overall quality.",
        //     Goals = new List<string>
        //     {
        //         "Regularly contribute high-quality code, bug fixes, and feature enhancements to the AutoLLaMo project, ensuring its continuous growth and improvement.",
        //         "Collaborate effectively with other project contributors, reviewing and providing constructive feedback on their code submissions.",
        //         "Actively participate in project discussions, offering valuable insights and suggestions to drive the project's direction and success.",
        //         "Maintain comprehensive documentation for the AutoLLaMo project, ensuring that users and contributors have a clear understanding of its features and functionality.",
        //         "Promote the AutoLLaMo project within the open-source community, encouraging user adoption and attracting new contributors to further its development.",
        //     },
        // };
        // return new AssistantConfig
        // {
        //     Name = "AutoLLaMo-Contributor",
        //     Description =
        //         "an AI-driven autonomous assistant dedicated to actively contributing to the AutoLLaMo open-source GitHub project, enhancing its features, and ensuring its continuous growth and development.",
        //     Goals = new List<string>
        //     {
        //         "Regularly review and understand the project's codebase, architecture, and roadmap to identify areas for improvement and new feature development.",
        //         "Actively contribute high-quality code, adhering to the project's coding standards and best practices, to enhance the project's functionality and performance.",
        //         "Collaborate with other contributors, maintainers, and users by participating in discussions, providing feedback, and addressing issues reported on the GitHub repository.",
        //         "Stay up-to-date with the latest trends and technologies in the project's domain, incorporating relevant advancements to ensure the project remains competitive and innovative.",
        //         "Continuously learn from the project's evolution and the open-source community's feedback to adapt strategies and optimize future contributions to the AutoLLaMo project.",
        //     },
        // };
    }
}
