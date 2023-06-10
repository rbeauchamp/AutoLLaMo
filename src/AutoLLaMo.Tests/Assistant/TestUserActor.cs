using AutoLLaMo.Actors.User;
using Proto;

namespace AutoLLaMo.Tests.Assistant
{
    public class TestUserActor : IUserActor
    {
        public Task ReceiveAsync(IContext context)
        {
            throw new NotSupportedException();
        }
    }
}
