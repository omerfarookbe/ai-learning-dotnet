using Google.GenAI;
using Google.GenAI.Types;
using AiLearning.Api.Models;
using AiLearning.Api.Services.Interface;

namespace AiLearning.Api.Services.Implementation;

public class GeminiService : ILlmService
{
    private readonly Client _client;
    private const string Model = "gemini-2.0-flash";

    public GeminiService(IConfiguration config)
    {
        var apiKey = config["LlmProviders:Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini API key not configured");

        _client = new Client(apiKey: apiKey);
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request)
    {
        var contents = new List<string>();

        // Gemini doesn't have a separate system role in this SDK — prepend it as context
        var prompt = string.IsNullOrWhiteSpace(request.SystemPrompt)
            ? request.Message
            : $"{request.SystemPrompt}\n\n{request.Message}";

        var response = await _client.Models.GenerateContentAsync(
            model: Model,
            contents: prompt,
            config: new GenerateContentConfig
            {
                MaxOutputTokens = request.MaxTokens,
                Temperature = request.Temperature
            }
        );

        var text = response.Candidates?[0].Content?.Parts?[0].Text ?? string.Empty;
        var inputTokens = (int)(response.UsageMetadata?.PromptTokenCount ?? 0);
        var outputTokens = (int)(response.UsageMetadata?.CandidatesTokenCount ?? 0);

        return new ChatResponse
        {
            Provider = $"Gemini ({Model})",
            Content = text,
            InputTokens = inputTokens,
            OutputTokens = outputTokens
        };
    }
}