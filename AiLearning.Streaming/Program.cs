using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
    ?? throw new InvalidOperationException("Set ANTHROPIC_API_KEY");

var client = new AnthropicClient(apiKey);

var parameters = new MessageParameters
{
    Model = AnthropicModels.Claude4Sonnet,
    MaxTokens = 512,
    Stream = true,
    System = new List<SystemMessage>
    {
        new SystemMessage("You are a concise market research analyst.")
    },
    Messages = new List<Message>
    {
        new Message(RoleType.User,
            "Explain why consumer sentiment analysis matters for brands. " +
            "Write 3 short paragraphs.")
    }
};

Console.WriteLine("Streaming response:\n");
Console.WriteLine("-".PadRight(60, '-'));

await foreach (var chunk in client.Messages.StreamClaudeMessageAsync(parameters))
{
    if (chunk.Delta?.Text != null)
    {
        Console.Write(chunk.Delta.Text);
    }
}

Console.WriteLine("\n" + "-".PadRight(60, '-'));