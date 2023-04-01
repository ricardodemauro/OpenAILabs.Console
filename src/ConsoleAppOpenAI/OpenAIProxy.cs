using Microsoft.Extensions.Configuration;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleAppOpenAI
{
    public class OpenAIProxy
    {
        readonly OpenAIClient openAIClient;

        readonly List<ChatCompletionMessage> _messages;

        readonly ChatCompletionMessage _systemMessage;

        public OpenAIProxy([NotNull] IConfiguration configuration, string systemConfig)
        {
            var openAIConfigurations = new OpenAIConfigurations
            {
                ApiKey = configuration["OpenAI:ApiKey"],
                OrganizationId = configuration["OpenAI:OrganizationId"]
            };

            openAIClient = new OpenAIClient(openAIConfigurations);

            _messages = new List<ChatCompletionMessage>();

            _systemMessage = new ChatCompletionMessage() { Content = systemConfig, Role = "system" };

        }

        void StackMessages(params ChatCompletionMessage[] message)
        {
            _messages.AddRange(message);
        }

        ChatCompletionMessage[] ToCompletitionMessage(ChatCompletionChoice[] choices)
            => choices.Select(x => x.Message).ToArray();

        public Task<ChatCompletionMessage[]> SendChatMessage(string message)
        {
            var chatMsg = new ChatCompletionMessage() { Content = message, Role = "user" };
            return SendChatMessage(chatMsg);
        }

        public async Task<ChatCompletionMessage[]> SendChatMessage(ChatCompletionMessage message)
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

            var messages = ToCompletitionMessage(choices);

            StackMessages(messages);

            return messages;
        }
    }
}
