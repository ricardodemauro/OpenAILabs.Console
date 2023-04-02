using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace ConsoleAppOpenAI;

public interface IOpenAIProxy
{
    Task<ChatCompletionMessage[]> SendChatMessage(string message);
}
