using AiLearning.Api.Models;
using AiLearning.Api.Services.Interface;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace AiLearning.Api.Services.Implementation
{
    public class ClaudeService : ILlmService
    {
        private readonly AnthropicClient _client;
        public ClaudeService(IConfiguration configuration)
        {
            var apiKey = configuration["LlmProviders:Anthropic:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Services.Exceptions.ProviderConfigurationException("Anthropic API key is not configured");
            }

            try
            {
                _client = new Anthropic.SDK.AnthropicClient(apiKey);
            }
            catch (Exception ex)
            {
                throw new Services.Exceptions.ProviderRuntimeException("Failed to initialize Anthropic client", ex);
            }
        }

        public async Task<ChatResponse> ChatAsync(ChatRequest request)
        {
            var messages = new List<Message>
            {
                new Message(RoleType.User, request.Message)
            };

            var parameters = new MessageParameters
            {
                Model = AnthropicModels.Claude45Sonnet,
                MaxTokens = request.MaxTokens,
                Temperature = (decimal)request.Temperature,
                Messages = messages
            };

            if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                parameters.System = new List<SystemMessage>
                {
                    new SystemMessage(request.SystemPrompt)
                };
            }

            try
            {
                var response = await _client.Messages.GetClaudeMessageAsync(parameters);

                return new ChatResponse
                {
                    Provider = "Claude",
                    Content = response.Content.OfType<TextContent>().First().Text,
                    InputTokens = (int)response.Usage.InputTokens,
                    OutputTokens = (int)response.Usage.OutputTokens
                };
            }
            catch (Exception ex)
            {
                throw new Services.Exceptions.ProviderRuntimeException("Anthropic provider call failed", ex);
            }
        }
    }
}
