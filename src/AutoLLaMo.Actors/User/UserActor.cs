using System.ComponentModel;
using System.Diagnostics;
using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
using Proto;
using Proto.Extensions;
using Spectre.Console;

namespace AutoLLaMo.Actors.User
{
    public class UserActor : IActor
    {
        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                // Start the interaction loop between the user and assistant
                case Started:
                    context.Send<AssistantActor>(new ConfigureAssistant());
                    break;
                // Commands
                case RequestConfirmation requestConfirmation:
                    await RequestConfirmationAsync(
                        context,
                        requestConfirmation);
                    break;
                case RequestValue requestValue:
                    await RequestValueAsync(
                        context,
                        requestValue);
                    break;
                // Events
                case AssistantConfigured assistantConfigured:
                    await HandleEventAsync(
                        context,
                        assistantConfigured);
                    break;
                case AssistantResponded assistantResponded:
                    await HandleEventAsync(
                        context,
                        assistantResponded);
                    break;
                case NextCommandExecuted nextCommandExecuted:
                    await HandleEventAsync(
                        context,
                        nextCommandExecuted);
                    return;
                default:
                    throw new InvalidStateException(
                        $"{GetType().Name} does not handle messages of type {context.Message.GetMessageTypeName()}");
            }
        }

        private static Task HandleEventAsync(
            IContext context,
            NextCommandExecuted nextCommandExecuted)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine(
                $"[olive]Command {nextCommandExecuted.NextCommandApproved.NextCommand.Name} returned:[/]");
            AnsiConsole.WriteLine($"{nextCommandExecuted.Response.Output?.Summary}");

            // Continue processing
            return AnsiConsole.Status().Spinner(Spinner.Known.Default).StartAsync(
                "Thinking...",
                async _ =>
                {
                    await context.RequestAsync<AssistantActor>(
                        new ChatWithOpenAI(NextCommandExecuted: nextCommandExecuted));
                });
        }

        private static async Task HandleEventAsync(
            IContext context,
            AssistantResponded assistantResponded)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[olive]THOUGHTS: [/]");
            AnsiConsole.WriteLine(
                $"{assistantResponded.AssistantResponse.ThoughtProcess.Thoughts}");
            AnsiConsole.Markup("[olive]REASONING: [/]");
            AnsiConsole.WriteLine(
                $"{assistantResponded.AssistantResponse.ThoughtProcess.Reasoning}");
            AnsiConsole.MarkupLine("[olive]PLAN: [/]");
            foreach (var plan in assistantResponded.AssistantResponse.ThoughtProcess.Plan)
            {
                AnsiConsole.WriteLine($"  {plan}");
            }

            AnsiConsole.Markup("[olive]CRITICISM: [/]");
            AnsiConsole.WriteLine(
                $"{assistantResponded.AssistantResponse.ThoughtProcess.CritiqueOfPlan}");

            AnsiConsole.Markup("[teal]NEXT ACTION: [/]");
            Debug.Assert(
                assistantResponded.AssistantResponse.NextCommand != null,
                "assistantResponse.NextCommand != null");
            DisplayToUser(assistantResponded.AssistantResponse.NextCommand);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[teal]Authorize this command?[/]");

            await ConfirmOrExitAsync(
                context,
                defaultValue: false);

            AnsiConsole.MarkupLine(
                "[purple]-=-=-=-=-=-=-= COMMAND AUTHORIZED BY USER -=-=-=-=-=-=-=[/]");

            await AnsiConsole.Status().Spinner(Spinner.Known.Default).StartAsync(
                "Executing the command...",
                async _ =>
                {
                    Debug.Assert(
                        assistantResponded.AssistantResponse.NextCommand != null,
                        "assistantResponse.NextCommand != null");
                    await context.RequestAsync<AssistantActor>(
                        new NextCommandApproved(
                            assistantResponded.AssistantResponse.NextCommand,
                            assistantResponded));
                });
        }

        private static async Task HandleEventAsync(
            IContext context,
            AssistantConfigured assistantConfigured)
        {
            AnsiConsole.MarkupLine(
                $"[olive]{assistantConfigured.AssistantConfig.Name} here, at your service![/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[olive]Name: [/]");
            AnsiConsole.WriteLine(assistantConfigured.AssistantConfig.Name);
            AnsiConsole.Markup("[olive]Role: [/]");
            AnsiConsole.WriteLine(assistantConfigured.AssistantConfig.Role);
            AnsiConsole.MarkupLine("[olive]Goals: [/]");
            foreach (var goal in assistantConfigured.AssistantConfig.Goals)
            {
                AnsiConsole.Markup("[olive]- [/]");
                AnsiConsole.WriteLine($"{goal}");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[teal]Ready for me to get started?[/]");

            await ConfirmOrExitAsync(
                context,
                defaultValue: true);

            await AnsiConsole.Status().Spinner(Spinner.Known.Default).StartAsync(
                "Thinking...",
                async _ =>
                {
                    await context.RequestAsync<AssistantActor>(
                        new ChatWithOpenAI(NextCommandExecuted: null));
                });
        }

        private static async Task RequestConfirmationAsync(
            IContext context,
            RequestConfirmation requestConfirmation)
        {
            DisplayToUser(
                requestConfirmation,
                "blue");

            var isConfirmed = await ConfirmOrExitAsync(context);

            var confirmation = new Confirm(requestConfirmation)
            {
                IsConfirmed = isConfirmed,
            };

            context.Send<AssistantActor>(confirmation);
        }

        private static async Task RequestValueAsync(IContext context, RequestValue requestValue)
        {
            DisplayToUser(
                requestValue,
                "teal");

            AnsiConsole.Write("> ");
            var input = Console.ReadLine().TrimToNull();

            if (ShouldEndProgram(input))
            {
                await ExitAsync(context);
            }

            var provideValue = new ProvideValue(requestValue)
            {
                Value = input,
            };

            context.Send<AssistantActor>(provideValue);
        }

        private static async Task<bool> ConfirmOrExitAsync(
            IContext context,
            bool defaultValue = false)
        {
            var isConfirmed = AnsiConsole.Confirm(
                string.Empty,
                defaultValue);

            if (!isConfirmed)
            {
                await ExitAsync(context);
            }

            AnsiConsole.WriteLine();
            return isConfirmed;
        }

        private static async Task ExitAsync(IContext context)
        {
            try
            {
                await context.System.ShutdownAsync("User ended the program");
            }
            finally
            {
                Environment.Exit(exitCode: 0);
            }
        }

        public static void DisplayToUser(ChatMessage chatMessage, string color)
        {
            AnsiConsole.WriteLine();

            var lines = chatMessage.Lines;

            if (lines == null || lines.Count == 0)
            {
                throw new InvalidAsynchronousStateException(
                    $"{nameof(chatMessage)} must have at least one line");
            }

            foreach (var line in lines)
            {
                var isWelcome = line.Contains(
                    "Welcome",
                    StringComparison.OrdinalIgnoreCase);

                AnsiConsole.MarkupLine(isWelcome ? $"[green]{line}[/]" : $"[{color}]{line}[/]");
            }
        }

        public static void DisplayToUser(NextCommand command)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write("  Command:");
            AnsiConsole.MarkupLine($" [teal]{command.Name}[/]");
            AnsiConsole.WriteLine("  Value:");
            AnsiConsole.MarkupLine($" [teal]{command.Json.EscapeMarkup()}[/]");
        }

        private static bool ShouldEndProgram(string? text)
        {
            return string.Equals(
                       text,
                       "exit",
                       StringComparison.OrdinalIgnoreCase)
                   || string.Equals(
                       text,
                       "quit",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
