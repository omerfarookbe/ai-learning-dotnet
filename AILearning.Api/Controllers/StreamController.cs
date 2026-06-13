using AiLearning.Api.Models;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AiLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly AnthropicClient _client;

    public StreamController(IConfiguration config)
    {
        var apiKey = config["LlmProviders:Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("Anthropic API key not configured");

        _client = new AnthropicClient(apiKey);
    }

    [HttpPost("chat")]
    public async Task StreamChat([FromBody] ChatRequest request)
    {
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        var parameters = new MessageParameters
        {
            Model = AnthropicModels.Claude4Sonnet,
            MaxTokens = request.MaxTokens,
            Stream = true,
            Messages = new List<Message>
            {
                new Message(RoleType.User, request.Message)
            }
        };

        if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
        {
            parameters.System = new List<SystemMessage>
            {
                new SystemMessage(request.SystemPrompt)
            };
        }

        await foreach (var chunk in _client.Messages.StreamClaudeMessageAsync(parameters))
        {
            if (chunk.Delta?.Text != null)
            {
                await Response.WriteAsync($"data: {chunk.Delta.Text}\n\n");
                await Response.Body.FlushAsync();
            }
        }

        await Response.WriteAsync("data: [DONE]\n\n");
        await Response.Body.FlushAsync();
    }
}