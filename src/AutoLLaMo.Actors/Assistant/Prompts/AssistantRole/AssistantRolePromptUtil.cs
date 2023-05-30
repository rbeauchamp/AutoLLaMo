using AutoLLaMo.Model;
using Rystem.OpenAi.Chat;

namespace AutoLLaMo.Actors.Assistant.Prompts.AssistantRole;

public static class AssistantRolePromptUtil
{
    public static ChatMessage GenerateAssistantRolePrompt(this AssistantConfig assistantConfig)
    {
        return new ChatMessage
        {
            Role = ChatRole.System,
            Content = assistantConfig.GenerateContent().FormatContent(),
        };
    }
}
