namespace AiLearning.Rag;

public class DocumentStore
{
    private readonly List<Document> _documents = new();
    private readonly EmbeddingService _embeddingService;

    public DocumentStore(EmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task AddAsync(string id, string content, string source)
    {
        Console.WriteLine($"  Embedding: \"{content[..Math.Min(60, content.Length)]}...\"");

        var vector = await _embeddingService.EmbedAsync(content);

        _documents.Add(new Document
        {
            Id = id,
            Content = content,
            Source = source,
            Vector = vector
        });
    }

    public async Task<List<(Document doc, float score)>> SearchAsync(string query, int topK = 3)
    {
        // Embed the query — same space as the documents
        var queryVector = await _embeddingService.EmbedAsync(query);

        // Score every document against the query
        return _documents
            .Select(doc => (doc, score: EmbeddingService.CosineSimilarity(queryVector, doc.Vector)))
            .OrderByDescending(x => x.score)
            .Take(topK)
            .ToList();
    }
}