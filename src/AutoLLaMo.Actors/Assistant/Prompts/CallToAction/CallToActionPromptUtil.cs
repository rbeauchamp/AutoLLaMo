using Rystem.OpenAi.Chat;
using ChatMessage = Rystem.OpenAi.Chat.ChatMessage;

namespace AutoLLaMo.Actors.Assistant.Prompts.CallToAction;

public static class CallToActionPromptUtil
{
    public static ChatMessage GenerateCallToActionPrompt()
    {
        return new ChatMessage
        {
            Role = ChatRole.User,
            Content = "Determine which next command to execute from the list of available commands, and respond with your thought process using the format specified above:",
        };
    }
}