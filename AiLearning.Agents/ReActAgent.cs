using AiLearning.Agents.Tools;
using Anthropic.SDK;
using Anthropic.SDK.Common;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.Linq;

namespace AiLearning.Agents;

public class ReActAgent
{
    private readonly AnthropicClient _client;
    private readonly ToolExecutor _executor;
    private const int MaxIterations = 10;

    public ReActAgent(string apiKey)
    {
        _client = new AnthropicClient(apiKey);
        _executor = new ToolExecutor();
    }

    public async Task<string> RunAsync(string userMessage)
    {
        Console.WriteLine($"\nUSER: {userMessage}");
        Console.WriteLine("-".PadRight(60, '-'));

        var messages = new List<Message>
        {
            new Message(RoleType.User, userMessage)
        };

        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            Console.WriteLine($"\n[Iteration {iteration + 1}] Calling Claude...");

            var response = await _client.Messages.GetClaudeMessageAsync(
                new MessageParameters
                {
                    Model = AnthropicModels.Claude4Sonnet,
                    MaxTokens = 1024,
                    Tools = ToolDefinitions.All,
                    Messages = messages,
                    System = new List<SystemMessage>
                    {
                        new SystemMessage(
                            "You are a market research assistant for the Suzy platform. " +
                            "Use the available tools to look up survey data and answer accurately. " +
                            "Always use the calculator tool for any arithmetic. " +
                            "Always cite the data source in your final answer.")
                    }
                });

            // Add assistant response to history (flatten content to string)
            messages.Add(new Message(RoleType.Assistant, FlattenContents(response.Content)));

            if (response.StopReason == "end_turn")
            {
                var finalText = response.Content
                    .OfType<TextContent>()
                    .FirstOrDefault()?.Text ?? "No response";

                Console.WriteLine($"\nFINAL ANSWER: {finalText}");
                return finalText;
            }

            if (response.StopReason == "tool_use")
            {
                var toolUseBlocks = response.Content.OfType<ToolUseContent>().ToList();
                var toolResults = new List<ContentBase>();

                foreach (var toolUse in toolUseBlocks)
                {
                    var inputElement = ToJsonElement(toolUse.Input);
                    var result = _executor.Execute(toolUse.Name, inputElement);
                    Console.WriteLine($"  [TOOL RESULT] {result}");

                    toolResults.Add(new ToolResultContent
                    {
                        ToolUseId = toolUse.Id,
                        Content = new List<ContentBase>
                        {
                            new TextContent { Text = result }
                        }
                    });
                }

                // Add tool results back into the conversation as a user message
                messages.Add(new Message(RoleType.User, FlattenContents(toolResults)));
                continue;
            }

            break;
        }

        return "Agent exceeded maximum iterations";
    }

    private static string FlattenContents(IEnumerable<Anthropic.SDK.Messaging.ContentBase> contents)
    {
        var sb = new StringBuilder();
        foreach (var c in contents)
        {
            switch (c)
            {
                case Anthropic.SDK.Messaging.TextContent t:
                    if (!string.IsNullOrEmpty(t.Text)) sb.Append(t.Text);
                    break;
                case Anthropic.SDK.Messaging.ToolUseContent tu:
                    sb.Append($"[ToolUse: {tu.Name} id={tu.Id} input={tu.Input}]");
                    break;
                case Anthropic.SDK.Messaging.ToolResultContent tr:
                    sb.Append($"[ToolResult for {tr.ToolUseId}]");
                    if (tr.Content != null)
                        sb.Append(FlattenContents(tr.Content));
                    break;
                default:
                    sb.Append(c.ToString());
                    break;
            }
            sb.Append(' ');
        }

        return sb.ToString().Trim();
    }

    private static JsonElement ToJsonElement(object? node)
    {
        if (node is JsonNode jnode)
        {
            using var doc = JsonDocument.Parse(jnode.ToJsonString());
            return doc.RootElement.Clone();
        }

        // Fallback: serialize then parse
        var json = JsonSerializer.Serialize(node);
        using var doc2 = JsonDocument.Parse(json);
        return doc2.RootElement.Clone();
    }
}