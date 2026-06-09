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
            var apiKey = config["LlmProviders:OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Services.Exceptions.ProviderConfigurationException("OpenAI API key is not configured");
            }

            try
            {
                _client = new ChatClient(model: "gpt-4o-mini", apiKey);
            }
            catch (Exception ex)
            {
                throw new Services.Exceptions.ProviderRuntimeException("Failed to initialize OpenAI client", ex);
            }
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

            try
            {
                var response = await _client.CompleteChatAsync(messages, options);

                return new ChatResponse
                {
                    Provider = "OpenAI",
                    Content = response.Value.Content[0].Text,
                    InputTokens = response.Value.Usage.InputTokenCount,
                    OutputTokens = response.Value.Usage.OutputTokenCount
                };
            }
            catch (Exception ex)
            {
                throw new Services.Exceptions.ProviderRuntimeException("OpenAI provider call failed", ex);
            }
        }
    }
}
