using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace AiLearning.Rag;

public class RagEngine
{
    private readonly DocumentStore _store;
    private readonly AnthropicClient _claudeClient;

    public RagEngine(DocumentStore store, string anthropicApiKey)
    {
        _store = store;
        _claudeClient = new AnthropicClient(anthropicApiKey);
    }

    public async Task<string> AskAsync(string question, int topK = 3)
    {
        // Step 1 — retrieve relevant documents
        Console.WriteLine($"\nSearching for relevant documents...");
        var results = await _store.SearchAsync(question, topK);

        Console.WriteLine($"Top {topK} matches:");
        foreach (var (doc, score) in results)
            Console.WriteLine($"  [{score:F2}] ({doc.Source}) {doc.Content[..Math.Min(60, doc.Content.Length)]}...");

        // Step 2 — build context from retrieved documents
        var context = string.Join("\n\n", results.Select((r, i) =>
            $"[Source {i + 1}: {r.doc.Source}]\n{r.doc.Content}"));

        // Step 3 — build the prompt with context injected
        var prompt = $"""
            Answer the question using ONLY the context provided below.
            If the answer is not in the context, say "I don't have that information."
            Always cite which source you used.

            CONTEXT:
            {context}

            QUESTION:
            {question}
            """;

        // Step 4 — send to Claude
        var response = await _claudeClient.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = AnthropicModels.Claude4Sonnet,
                MaxTokens = 512,
                Messages = new List<Message>
                {
                    new Message(RoleType.User, prompt)
                }
            });

        return response.Content.OfType<TextContent>().First().Text;
    }
}