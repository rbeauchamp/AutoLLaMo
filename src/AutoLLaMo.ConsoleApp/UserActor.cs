using System.Diagnostics;
using AutoLLaMo.Actors.Assistant;
using AutoLLaMo.Actors.User;
using AutoLLaMo.Common;
using AutoLLaMo.Model;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Model.Messages.Events;
using Proto;
using Proto.Extensions;
using Spectre.Console;

namespace AutoLLaMo.ConsoleApp
{
    public class UserActor : IUserActor
    {
        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started started:
                    await HandleAsync(
                        context,
                        started);
                    return;
                case GetUserDesire getUserDesire:
                    await HandleAsync(
                        context,
                        getUserDesire);
                    return;
                case AssistantConfigured assistantConfigured:
                    await HandleAsync(
                        context,
                        assistantConfigured);
                    return;
                case AssistantResponded assistantResponded:
                    await HandleAsync(
                        context,
                        assistantResponded);
                    return;
                case NextCommandExecuted nextCommandExecuted:
                    await HandleAsync(
                        context,
                        nextCommandExecuted);
                    return;
                default:
                    await HandleAsync(context);
                    return;
            }
        }

        protected virtual Task HandleAsync(IContext context)
        {
            return Task.CompletedTask;
        }

        private static Task HandleAsync(IContext context, Started started)
        {
            context.Send<AssistantActor>(new ConfigureAssistant());

            return Task.CompletedTask;
        }

        private static Task HandleAsync(
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
                        new ChatWithOpenAI(nextCommandExecuted));
                });
        }

        private static async Task HandleAsync(
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

        private static async Task HandleAsync(
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

        protected virtual async Task HandleAsync(IContext context, GetUserDesire getUserDesire)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Welcome to AutoLLaMo![/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[teal]Enter nothing to load the default AutoLLaMo assistant[/]");
            AnsiConsole.MarkupLine("[teal]I want my assistant to: [/]");
            AnsiConsole.Write("> ");

            var input = Console.ReadLine().TrimToNull();

            if (ShouldEndProgram(input))
            {
                await ExitAsync(context);
            }

            await AnsiConsole.Status().Spinner(Spinner.Known.Default).StartAsync(
                "Thinking...",
                async _ =>
                {
                    var provideValue = new ProvideUserDesire(getUserDesire)
                    {
                        UserDesire = input,
                    };

                    await context.RequestAsync<AssistantActor>(provideValue);
                });
        }

        private static async Task ConfirmOrExitAsync(IContext context, bool defaultValue = false)
        {
            var isConfirmed = AnsiConsole.Confirm(
                string.Empty,
                defaultValue);

            if (!isConfirmed)
            {
                await ExitAsync(context);
            }

            AnsiConsole.WriteLine();
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
