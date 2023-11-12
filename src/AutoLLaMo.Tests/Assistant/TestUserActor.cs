using AutoLLaMo.ConsoleApp;
using AutoLLaMo.Model.Messages.Chats;
using AutoLLaMo.Plugins;
using Proto;

namespace AutoLLaMo.Tests.Assistant
{
    public class TestUserActor : UserActor
    {
        public List<Message> Messages { get; } = new();

        protected override Task HandleAsync(IContext context)
        {
            switch (context.Message)
            {
                case GetMessages:
                    context.Respond(Messages);
                    break;
            }

            return Task.CompletedTask;
        }

        protected override Task HandleAsync(IContext context, GetUserDesire getUserDesire)
        {
            Messages.Add(getUserDesire);

            return Task.CompletedTask;
        }
    }
}
