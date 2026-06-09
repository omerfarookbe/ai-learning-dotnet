using AiLearning.Api.Models;
using AiLearning.Api.Services.Interface;
using OpenAI.Chat;

namespace AiLearning.Api.Services.Implementation
{
    public class OpenAIService : ILlmService
    {
        private readonly ChatClient _client;

        public OpenAIService(IConfiguration config)
        {
            var apiKey = config["LlmProviders:OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI API key is not configured");
            _client = new ChatClient(model: "gpt-4o-mini", apiKey);
        }

        public async Task<ChatResponse> ChatAsync(ChatRequest request)
        {
            var messages = new List<OpenAI.Chat.ChatMessage>();

            if(!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                messages.Add(new SystemChatMessage(request.SystemPrompt));
            }

            messages.Add(new UserChatMessage(request.Message));

            var options = new ChatCompletionOptions()
            {
                MaxOutputTokenCount = request.MaxTokens,
                Temperature = request.Temperature
            };

            var response = await _client.CompleteChatAsync(messages, options);

            return new ChatResponse
            {
                Provider = "OpenAI",
                Content = response.Value.Content[0].Text,
                InputTokens = response.Value.Usage.InputTokenCount,
                OutputTokens = response.Value.Usage.OutputTokenCount
            };
        }
    }
}
