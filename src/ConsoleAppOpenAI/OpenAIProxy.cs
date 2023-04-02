using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace ConsoleAppOpenAI;

public class OpenAIProxy : IOpenAIProxy
{
    readonly OpenAIClient openAIClient;

    readonly List<ChatCompletionMessage> _messages;

    public OpenAIProxy(string apiKey, string organizationId)
    {
        var openAIConfigurations = new OpenAIConfigurations
        {
            ApiKey = apiKey,
            OrganizationId = organizationId
        };

        openAIClient = new OpenAIClient(openAIConfigurations);

        _messages = new List<ChatCompletionMessage>();
    }

    public void SetSystemMessage(string systemMessage)
    {
        var sysMsg = new ChatCompletionMessage() { Content = systemMessage, Role = "system" };
        _messages.Insert(0, sysMsg);
    }

    void StackMessages(params ChatCompletionMessage[] message)
    {
        _messages.AddRange(message);
    }

    static ChatCompletionMessage[] ToCompletionMessage(ChatCompletionChoice[] choices)
        => choices.Select(x => x.Message).ToArray();

    public Task<ChatCompletionMessage[]> SendChatMessage(string message)
    {
        var chatMsg = new ChatCompletionMessage() { Content = message, Role = "user" };
        return SendChatMessage(chatMsg);
    }

    async Task<ChatCompletionMessage[]> SendChatMessage(ChatCompletionMessage message)
    {
        StackMessages(message);

        var chatCompletion = new ChatCompletion
        {
            Request = new ChatCompletionRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = _messages.ToArray(),
                Temperature = 0.2,
                MaxTokens = 800
            }
        };

        ChatCompletion resultChatCompletion = await openAIClient.ChatCompletions.SendChatCompletionAsync(chatCompletion);

        var choices = resultChatCompletion.Response.Choices;

        var messages = ToCompletionMessage(choices);

        StackMessages(messages);

        return messages;
    }
}
